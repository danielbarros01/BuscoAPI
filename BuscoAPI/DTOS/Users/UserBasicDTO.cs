using BuscoAPI.DTOS.Worker;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS.Users
{
    public class UserBasicDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime Birthdate { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string Department { get; set; }
        public string City { get; set; }
        public string? Image { get; set; }
        public WorkerWithoutUser Worker { get; set; }
    }
}
