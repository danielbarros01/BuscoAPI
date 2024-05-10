using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BuscoAPI.DTOS.Users
{
    public class UserLoginDTO : IUserDto
    {
        [StringLength(20)]
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
