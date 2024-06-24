using BuscoAPI.Entities;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoMapper;
using BuscoAPI.Services;
using BuscoAPI.Helpers;
using BuscoAPI.DTOS.Users;
using System.Xml.Linq;

namespace BuscoAPI.Controllers
{
    [ApiController]
    [Route("/api/google")]
    public class GoogleLoginController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
 
        public GoogleLoginController(ApplicationDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("session")]
        public IActionResult Index()
        {
            return new ChallengeResult(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse", "GoogleLogin") // Where google responds back
                });
        }

        //Este endpoint es para la web
        [HttpGet]
        [Route("signin-google")]
        public async Task<ActionResult<UserToken>> GoogleResponse()
        {
            try
            {
                //Check authentication response as mentioned on startup file as o.DefaultSignInScheme = "External"
                var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (!authenticateResult.Succeeded) return Unauthorized(); // TODO: Handle this better.

                //Check if the redirection has been done via google or any other links
                if (authenticateResult.Principal.Identities.ToList()[0].AuthenticationType.ToLower() == "google")
                {
                    //check if principal value exists or not 
                    if (authenticateResult.Principal != null)
                    {
                        //get google account id for any operation to be carried out on the basis of the id
                        var googleAccountId = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
                        var name = authenticateResult.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
                        var surname = authenticateResult.Principal.FindFirst(ClaimTypes.Surname)?.Value;

                        //claim value initialization as mentioned on the startup file with o.DefaultScheme = "Application"
                        //var claimsIdentity = new ClaimsIdentity("Application");
                        if (authenticateResult.Principal != null)
                        {
                            var googleIdExists = await context.Users.AnyAsync(x => x.Google_id == googleAccountId);

                            //I create the user if it does not exist
                            if (!googleIdExists)
                            {
                                //Create user
                                var user = new User
                                {
                                    Email = email,
                                    Username = googleAccountId,
                                    Password = HashPassword.HashingPassword($"{Guid.NewGuid()}"),
                                    Name = name,
                                    Lastname = surname,
                                    Confirmed = true,
                                    Google_id = googleAccountId
                                };
                                context.Users.Add(user);
                                await context.SaveChangesAsync();
                            }

                            //Create user for BuildToken
                            var userInfo = new UserBasicInfoDTO { Email = email, Username = googleAccountId };
                            var userToken = await TokenHelper.BuildToken(userInfo, context, configuration);

                            return userToken;
                        }
                    }
                }

                return Unauthorized();
            }
            catch (Exception ex){
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        //Este endpoint es para android
        [HttpPost("signin")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserGoogleDTO userGoogle)
        {
            try
            {
                //verificar si existe el usuario
                var googleIdExists = await context.Users.AnyAsync(x => x.Google_id == userGoogle.GoogleId || x.Email == userGoogle.Email);

                //I create the user if it does not exist
                if (!googleIdExists)
                {
                    var usernameExists = await context.Users.AnyAsync(x => x.Username == userGoogle.Username);
                    
                    while (usernameExists){
                        userGoogle.Username = userGoogle.Username + Utility.GenerateRandomString(4);

                        usernameExists = await context.Users.AnyAsync(x => x.Username == userGoogle.Username);
                    }

                    //Create user
                    var user = new User
                    {
                        Email = userGoogle.Email,
                        Username = userGoogle.Username,
                        Password = HashPassword.HashingPassword($"{Guid.NewGuid()}"),
                        Confirmed = true,
                        Google_id = userGoogle.GoogleId
                    };
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                }

                //Create user for BuildToken
                var userInfo = new UserBasicInfoDTO { Email = userGoogle.Email, Username = userGoogle.Username };
                var userToken = await TokenHelper.BuildToken(userInfo, context, configuration);

                return userToken;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}