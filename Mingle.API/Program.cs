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


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 41943040; // 5 MB
});


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Mingle.Cors", policy =>
    {
        policy.WithOrigins("https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
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
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? throw new ArgumentNullException("JwtSettings:Issuer"),
        ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? throw new ArgumentNullException("JwtSettings:Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret")))
    };
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseCors("Mingle.Cors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


// SignalR Hubs
app.MapHub<ChatHub>("ChatHub");
app.MapHub<CallHub>("CallHub");
app.MapHub<NotificationHub>("NotificationHub");


app.Run();