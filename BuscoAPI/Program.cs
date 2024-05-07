
using BuscoAPI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));

//Configure MySQL
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseMySql(
        configuration["ConnectionStrings:MySqlConnection"],
        ServerVersion.AutoDetect(configuration["ConnectionStrings:MySqlConnection"])
    )
);


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
