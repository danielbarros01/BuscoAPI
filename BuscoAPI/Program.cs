
using AutoMapper;
using BuscoAPI;
using BuscoAPI.Controllers;
using BuscoAPI.Helpers;
using BuscoAPI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services
    .AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); //Esto indica que se debe ignorar cualquier referencia circular o relación cíclica durante la serialización JSON

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient(); //para consumir apis
builder.Services.AddHttpClient<SNDGService>(); //para consumir api de ubicaciones


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

builder.WebHost.UseUrls("http://localhost:5029", "http://192.168.100.7:5029", "http://*:5029");



var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var sndgService = services.GetRequiredService<SNDGService>();
    var mapper = services.GetRequiredService<IMapper>();

    DbInitializer.SeedCategoriesAndProfessions(context);
    //await DbInitializer.SeedUsers(context, sndgService);
    //await DbInitializer.SeedWorkers(context, mapper);
    //await DbInitializer.SeedProposals(context);
}


// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
