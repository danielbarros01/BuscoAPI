
using AutoMapper;
using BuscoAPI;
using BuscoAPI.Controllers;
using BuscoAPI.Helpers;
using BuscoAPI.RealTime;
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

builder.Services
    .AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); //Esto indica que se debe ignorar cualquier referencia circular o relación cíclica durante la serialización JSON

builder.Services.AddSignalR();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient(); //to consume api
builder.Services.AddHttpClient<SNDGService>(); //to consume SNDG Service api


builder.Services.AddTransient<IFileStore, LocalFileStore>();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseMySql(
        configuration["ConnectionStrings:MySqlConnection"],
        ServerVersion.AutoDetect(configuration["ConnectionStrings:MySqlConnection"])
    )
);

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
            ValidateIssuer = false, 
            ValidateAudience = false, 
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

builder.Services.AddScoped<IEmailService, EmailService>();
builder.WebHost.UseUrls("http://localhost:5029", "http://192.168.1.73:5029", "http://*:5029");

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

app.UseRouting();

app.UseAuthorization();

app.MapControllers();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chathub");
    endpoints.MapHub<NotificationHub>("/notificationhub");
});

app.Run();
