using System.ComponentModel.DataAnnotations;

namespace BuscoAPI.DTOS.Users
{
    public class UserCreationDTO : IUserDto
    {
        [Required]
        [StringLength(20)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
