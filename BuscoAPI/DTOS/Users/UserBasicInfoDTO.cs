using System.ComponentModel.DataAnnotations;

namespace BuscoAPI.DTOS.Users
{
    public class UserBasicInfoDTO : IUserDto
    {
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
