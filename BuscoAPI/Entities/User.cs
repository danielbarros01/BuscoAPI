using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BuscoAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Lastname { get; set; }
        [Required]
        [StringLength(20)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        public DateTime Birthdate { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string Department { get; set; }
        public string City { get; set; }
        [AllowNull]
        [Column("Image_path")]
        public string? Image { get; set; }
        [Column("Verification_code")]
        public int? VerificationCode { get; set; }
        public bool? Confirmed { get; set; } = false;
        public string? Google_id { get; set; }

        public Worker? Worker { get; set; }
    }
}
