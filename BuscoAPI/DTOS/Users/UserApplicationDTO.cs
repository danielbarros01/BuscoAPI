using BuscoAPI.DTOS.Worker;

namespace BuscoAPI.DTOS.Users
{
    public class UserApplicationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string? Image { get; set; }
    }
}
