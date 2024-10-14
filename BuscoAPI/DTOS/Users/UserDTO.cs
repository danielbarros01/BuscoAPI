using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using BuscoAPI.DTOS.Worker;

namespace BuscoAPI.DTOS.Users
{
    public class UserDTO : IUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime Birthdate { get; set; }
        public double Latitude {  get; set; }
        public double Longitude {  get; set; }
        public int? VerificationCode { get; set; }
        public bool? Confirmed { get; set; } = false;
        [Column("google_id")]
        public string? GoogleId { get; set; }

        public WorkerWithQualification? Worker { get; set; }

    }
}
