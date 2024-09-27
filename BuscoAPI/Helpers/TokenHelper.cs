using BuscoAPI.DTOS.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BuscoAPI.Helpers
{
    public class TokenHelper
    {
        public static async Task<UserToken> BuildToken<T>(T userCreation, ApplicationDbContext context, 
            IConfiguration config) where T:IUserDto
        {
            var claims = new List<Claim>(){};

            //Si existen email y username
            if(!string.IsNullOrEmpty(userCreation.Email)){
                claims.Add(new Claim(ClaimTypes.Email, userCreation.Email));
            }

            if (!string.IsNullOrEmpty(userCreation.Username)){
                claims.Add(new Claim(ClaimTypes.Name, userCreation.Username));
            }

            var user = await context.Users.FirstAsync(x => x.Email == userCreation.Email || x.Username == userCreation.Username);
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.Username.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMonths(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
