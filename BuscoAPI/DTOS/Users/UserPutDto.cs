using BuscoAPI.Entities;
using BuscoAPI.Validations;
using NetTopologySuite.Geometries;
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
        [Range(-90,90)]
        [Required]
        public double Latitude { get; set; }
        [Range(-180, 180)]
        [Required]
        public double Longitude { get; set; }
    }
}
