using BuscoAPI.Validations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BuscoAPI.DTOS.Users
{
    public class UserPutDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Lastname { get; set; }
        [Required]
        [MinDate(Year = 1950, Month = 1, Day = 1, ErrorMessage = "The date must be after January 1, 1950")]
        [MaxDate(YearsAgo = -18, ErrorMessage = "Must be at least 18 years old")]
        public DateTime Birthdate { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        [Phone]
        public string Telephone { get; set; }
    }
}
