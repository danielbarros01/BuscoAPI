using System.Security.Claims;

namespace BuscoAPI.Helpers
{
    public class UtilItyAuth
    {
        public static int? GetUserIdFromClaims(HttpContext httpContext)
        {
            var userIdClaim = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return null;
            }

            return userId;
        }
    }
}
