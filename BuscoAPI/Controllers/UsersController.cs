using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using BuscoAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using BuscoAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using BuscoAPI.DTOS.Users;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BuscoAPI.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly IEmailService emailService;
        private readonly IFileStore fileStore;
        private readonly string container = "users"; //folder name

        public UsersController(ApplicationDbContext context, IConfiguration configuration,
            IMapper mapper, IEmailService emailService, IFileStore fileStore)
        {
            this.context = context;
            this.configuration = configuration;
            this.mapper = mapper;
            this.emailService = emailService;
            this.fileStore = fileStore;
        }


        [HttpPost("create")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserCreationDTO userCreation)
        {
            try{
                var userMailExists = await context.Users.AnyAsync(x => x.Email == userCreation.Email);
                var userUsernameExists = await context.Users.AnyAsync(x => x.Username == userCreation.Username);

                if (userMailExists || userUsernameExists)
                {
                    var errors = new List<object>();

                    if (userMailExists)
                        errors.Add(new ErrorInfo { Field = "email", Message = $"The email {userCreation.Email} already exists." });

                    if (userUsernameExists)
                        errors.Add(new ErrorInfo { Field = "username", Message = $"The username {userCreation.Username} already exists." });

                    return BadRequest(errors);
                }

                //I created the random number to validate the email
                var verificationCode = Helpers.Utility.RandomNumber(4);

                //Create user with Hash for password
                var user = new User
                {
                    Email = userCreation.Email,
                    Password = HashPassword.HashingPassword(userCreation.Password),
                    Username = userCreation.Username,
                    VerificationCode = verificationCode
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                var userToken = await TokenHelper.BuildToken(userCreation, context, configuration);

                //Send code to user's email
                var emailReq = SendEmails.BuildEmailVerificationCode(verificationCode);
                emailReq.ToEmail = userCreation.Email;
                emailService.SendEmail(emailReq);

                //Return the token to the client
                return userToken;
            }catch(Exception ex){
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserLoginDTO user)
        {
            try{
                var userDb = await context.Users.FirstOrDefaultAsync(x => x.Email == user.Email || x.Username == user.Username);

                if (userDb == null) return Unauthorized();

                //verificar si es correcto el password
                var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(user.Password, userDb.Password);
            
                if (!isPasswordCorrect)
                {
                    return Unauthorized();
                }
            
                var userToken = await TokenHelper.BuildToken(user, context, configuration);
                return userToken;
            }catch (Exception ex){
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPatch("confirm-register-code")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ConfirmCode([FromBody] VerificationCodeDTO verificationCode)
        {
            try{
                //Verify that the user exists
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == Int32.Parse(userId));
                if(user == null) return Unauthorized();

                if(verificationCode.Code != user.VerificationCode)
                {
                    return BadRequest(new ErrorInfo{ Field = "Code", Message = "The code is incorrect"});
                }

                //User confirmed a the code is removed 
                user.Confirmed = true;
                user.VerificationCode = null;

                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put([FromForm] UserPutDto user)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var userDb = await context.Users.FirstOrDefaultAsync(x => x.Id == Int32.Parse(userId));
                if (userDb == null) return Unauthorized();
                if ((bool)!userDb.Confirmed) {
                    return Unauthorized(new ErrorInfo { Field = "Confirmed", Message = "You must confirm your account" });
                }

                //Para que solo se modifiquen los campos que son distintos entre user y userDb
                userDb = mapper.Map(user, userDb);

                context.Entry(userDb).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
        {
            var users = await context.Users.ToListAsync();
            return Ok(users);
        }
    }
}
