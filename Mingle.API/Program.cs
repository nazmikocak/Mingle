using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Concrete;
using Mingle.DataAccess.Configurations;
using Mingle.Services.Abstract;
using Mingle.Services.Concrete;
using Mingle.Services.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<FirebaseConfiguration>();
builder.Services.AddScoped<CloudinaryConfiguration>();




// Services
builder.Services.AddScoped<IAuthService, AuthService>();






// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();