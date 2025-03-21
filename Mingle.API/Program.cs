using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mingle.API.Hubs;
using Mingle.Core.Abstract;
using Mingle.Core.Concrete;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Concrete;
using Mingle.DataAccess.Configurations;
using Mingle.Services.Abstract;
using Mingle.Services.Concrete;
using Mingle.Services.Mapping;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();


// SignalR
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 41943040; // 5 MB
});


// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("Mingle.Cors", policy =>
    {
        policy.WithOrigins("https://mingleweb.netlify.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => true);
    });
});


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// Dependency Injection
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddSingleton<IJwtManager, JwtManager>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<ICallRepository, CallRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<ICloudRepository, CloudRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ICallService, CallService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IGenerativeAiService, GenerativeAiService>();


// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));


// Configurations
builder.Services.AddSingleton<FirebaseConfig>();
builder.Services.AddSingleton<CloudinaryConfig>();
builder.Services.AddSingleton<GeminiConfig>();
builder.Services.AddSingleton<HuggingFaceConfig>();


// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:issuer"] ?? throw new ArgumentNullException("JwtSettings:issuer"),
        ValidAudience = builder.Configuration["JwtSettings:audience"] ?? throw new ArgumentNullException("JwtSettings:audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:secret"] ?? throw new ArgumentNullException("JwtSettings:secret")))
    };
});



var app = builder.Build();


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var errorFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (errorFeature != null)
        {
            var errorMessage = errorFeature.Error.Message;
            var stackTrace = errorFeature.Error.StackTrace;

            var errorResponse = new { error = errorMessage, details = stackTrace };
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    });
});


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseCors("Mingle.Cors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


// SignalR Hubs
app.MapHub<ChatHub>("hub/Chat");
app.MapHub<CallHub>("hub/Call");
app.MapHub<NotificationHub>("hub/Notification");


app.Run();