
using BuscoAPI.RealTime;
using BuscoAPI.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace BuscoAPI
{
    public class Startup
    {
        public IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); //Esto indica que se debe ignorar cualquier referencia circular o relación cíclica durante la serialización JSON

            services.AddSignalR();

            services.AddHttpContextAccessor();
            services.AddHttpClient(); //to consume api
            services.AddHttpClient<SNDGService>(); //to consume SNDG Service api


            services.AddTransient<IFileStore, LocalFileStore>();
            services.AddAutoMapper(typeof(Program));

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseMySql(
                    configuration["ConnectionStrings:MySqlConnection"],
                    ServerVersion.AutoDetect(configuration["ConnectionStrings:MySqlConnection"])
                )
            );

            services.AddAuthentication(options =>
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

            services.AddScoped<IEmailService, EmailService>();
        }
    
        public void Configure(WebApplication app)
        {
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
        }
    }
}
