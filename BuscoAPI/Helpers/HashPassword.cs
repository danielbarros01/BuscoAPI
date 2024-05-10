using BCrypt.Net;

namespace BuscoAPI.Helpers
{
    public class HashPassword
    {
        public static string HashingPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
