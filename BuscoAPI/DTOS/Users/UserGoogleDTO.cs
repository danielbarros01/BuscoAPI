using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS.Users
{
    public class UserGoogleDTO : IUserDto
    {
        [Required]
        [StringLength(20)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Column("google_id")]
        public string GoogleId { get; set; }
    }
}
