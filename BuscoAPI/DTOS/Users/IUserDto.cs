using System.ComponentModel.DataAnnotations;

namespace BuscoAPI.DTOS.Users
{
    public interface IUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
