using System.ComponentModel.DataAnnotations;

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
        public string Mail { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        public DateTime Birthdate { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        [Phone]
        public string Telephone { get; set; }
        [StringLength(250)]
        public string Description { get; set; }
        public string? Image_path { get; set; }
        public string? Token { get; set; }
        public bool? Confirmed { get; set; }
        public string? Google_id { get; set; }
    }
}
