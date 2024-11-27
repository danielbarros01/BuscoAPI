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
        private readonly string container = "users"; 

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
            try
            {
                var username = userCreation.Username.Trim().ToLower();
                var email = userCreation.Email.ToLower();

                var userMailExists = await context.Users.AnyAsync(x => x.Email == email);
                var userUsernameExists = await context.Users.AnyAsync(x => x.Username == username);

                if (userMailExists || userUsernameExists)
                {
                    var errors = new List<object>();

                    if (userMailExists)
                        errors.Add(new ErrorInfo { Field = "email", Message = $"El email {email} ya existe." });

                    if (userUsernameExists)
                        errors.Add(new ErrorInfo { Field = "username", Message = $"El nombre de usuario {username} ya existe." });

                    return BadRequest(errors);
                }

                //I created the random number to validate the email
                var verificationCode = Helpers.Utility.RandomNumber(4);

                var user = new User
                {
                    Email = email,
                    Password = HashPassword.HashingPassword(userCreation.Password),
                    Username = username,
                    VerificationCode = verificationCode
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                var userToken = await TokenHelper.BuildToken(userCreation, context, configuration);

                var emailReq = SendEmails.BuildEmailVerificationCode(verificationCode, "register", user.Username);
                emailReq.ToEmail = email;
                emailService.SendEmail(emailReq);

                return userToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserLoginDTO user)
        {
            try
            {
                var userDb = await context.Users.FirstOrDefaultAsync(x => x.Email == user.Email || x.Username == user.Username);

                if (userDb == null) return Unauthorized();

                var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(user.Password, userDb.Password);

                if (!isPasswordCorrect)
                {
                    return Unauthorized();
                }

                var userToken = await TokenHelper.BuildToken(user, context, configuration);
                return userToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPatch("confirm-register-code")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ConfirmCode([FromBody] VerificationCodeDTO verificationCode)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == Int32.Parse(userId));
                if (user == null) return Unauthorized();

                if (verificationCode.Code != user.VerificationCode)
                {
                    return BadRequest(new ErrorInfo { Field = "Code", Message = "El codigo es incorrecto" });
                }

                user.Confirmed = true;
                user.VerificationCode = null;

                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpPatch("confirm-password-code")]
        public async Task<ActionResult<UserToken>> ConfirmCodeForPassword([FromForm] String email, [FromForm] int code)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (user == null || code != user.VerificationCode)
                {
                    return BadRequest(new ErrorInfo { Field = "Code", Message = "El codigo es incorrecto" });
                }

                user.Confirmed = true;
                user.VerificationCode = null;

                await context.SaveChangesAsync();

                var userMapper = mapper.Map<User, UserDTO>(user);
                var userToken = await TokenHelper.BuildToken(userMapper, context, configuration);
                return userToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpPatch("resend-code")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ResendCode()
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var userDb = await context.Users.FirstOrDefaultAsync(x => x.Id == Int32.Parse(userId));
                if (userDb == null) return Unauthorized();

                var verificationCode = Helpers.Utility.RandomNumber(4);

                userDb.VerificationCode = verificationCode;

                var emailReq = SendEmails.BuildEmailVerificationCode(verificationCode, "register", userDb.Username);
                emailReq.ToEmail = userDb.Email;
                emailService.SendEmail(emailReq);

                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put([FromBody] UserPutDto user)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var userDb = await context.Users.FirstOrDefaultAsync(x => x.Id == Int32.Parse(userId));
                if (userDb == null) return Unauthorized();
                if ((bool)!userDb.Confirmed)
                {
                    return Unauthorized(new ErrorInfo { Field = "Confirmed", Message = "Debe confirmar su cuenta" });
                }

                userDb = mapper.Map(user, userDb);

                context.Entry(userDb).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<User>> GetMyProfile()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = await context.Users
                .Include(x => x.Worker)
                .ThenInclude(x => x.WorkersProfessions)
                .ThenInclude(x => x.Profession)
                .FirstOrDefaultAsync(x => x.Id == Int32.Parse(userId));

            if (user == null) return Unauthorized();

            var userMapper = mapper.Map<User, UserDTO>(user);
            return Ok(userMapper);
        }

        [HttpPatch("send-code")]
        public async Task<ActionResult> ResendCode([FromForm] String email)
        {
            try
            {
                var userDb = await context.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (userDb == null) return Ok();

                var verificationCode = Helpers.Utility.RandomNumber(4);
                userDb.VerificationCode = verificationCode;

                var emailReq = SendEmails.BuildEmailVerificationCode(verificationCode, "recover-password", userDb.Username);
                emailReq.ToEmail = userDb.Email;
                emailService.SendEmail(emailReq);

                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPatch("change-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ChangePassword([FromForm] String password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                return BadRequest(new ErrorInfo { Field = "Contraseña", Message = "La contraseña es un campo requerido de 6 caracteres minimo." });
            }

            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                if (userId == null) return Unauthorized(new ErrorInfo { Field = "Contraseña", Message = "Hubo un problema al validar el usuario." });

                var userDb = await context.Users.FirstOrDefaultAsync(x => x.Id == Int32.Parse(userId));
                if (userDb == null) return Unauthorized(new ErrorInfo { Field = "Contraseña", Message = "Hubo un problema al validar el usuario." });

                userDb.Password = HashPassword.HashingPassword(password);
                await context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPatch("me/image")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ChangePhotoProfile([FromForm] UserImageDto userImageDto)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                if (userId == null) return Unauthorized(new ErrorInfo { Field = "Contraseña", Message = "Hubo un problema al validar el usuario." });

                var userDb = await context.Users.FirstOrDefaultAsync(x => x.Id == Int32.Parse(userId));
                if (userDb == null) return Unauthorized(new ErrorInfo { Field = "Contraseña", Message = "Hubo un problema al validar el usuario." });

                //Guardar imagen
                using (var memoryStream = new MemoryStream())
                {
                    await userImageDto.Image.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray(); //datos en bytes
                    var extension = Path.GetExtension(userImageDto.Image.FileName);

                    if (userDb.Image != null)
                    {
                        userDb.Image = await fileStore.EditFile(
                            content,
                            extension,
                            container,
                            userDb.Image,
                            userImageDto.Image.ContentType
                         );
                    }
                    else
                    {
                        userDb.Image = await fileStore.SaveFile(
                           content,
                           extension,
                           container,
                           userImageDto.Image.ContentType
                        );
                    }

                }

                await context.SaveChangesAsync();

                return Ok(new { image = userDb.Image });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserBasicDTO>> GetUser(int id)
        {
            try
            {
                var user = await context.Users
                    .Include(x => x.Worker)
                    .ThenInclude(x => x.WorkersProfessions)
                    .ThenInclude(x => x.Profession)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (user == null) { return NotFound(); }
                var userMapper = mapper.Map<UserBasicDTO>(user);

                return userMapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = await context.Users.ToListAsync();
            return Ok(users);
        }
    }
}
