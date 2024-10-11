
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
var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);
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

startup.Configure(app);

app.Run();
