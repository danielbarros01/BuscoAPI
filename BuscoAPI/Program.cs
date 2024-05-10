
using BuscoAPI;
using BuscoAPI.Controllers;
using BuscoAPI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();


//Configure IFileStore
builder.Services.AddTransient<IFileStore, LocalFileStore>();

//Config automapper
builder.Services.AddAutoMapper(typeof(Program));

//Configure MySQL
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseMySql(
        configuration["ConnectionStrings:MySqlConnection"],
        ServerVersion.AutoDetect(configuration["ConnectionStrings:MySqlConnection"])
    )
);

//Configure JWT and Google Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = "External";
})
    .AddCookie("External")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Assuming you're issuing the JWT tokens yourself
            ValidateAudience = false, // Assuming you're not using an audience
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"])),
            ClockSkew = TimeSpan.Zero
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = configuration["Authentication:Google:ClientId"];
        options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    });

//Configure email service
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
