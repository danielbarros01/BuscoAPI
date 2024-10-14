using BuscoAPI.DTOS.Worker;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS.Users
{
    public class UserWithoutWorker
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
