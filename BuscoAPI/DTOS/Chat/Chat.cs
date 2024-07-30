using BuscoAPI.DTOS.Users;
using BuscoAPI.Entities;

namespace BuscoAPI.DTOS.Chat
{
    public class Chat
    {
        public User user { get; set; }
        public Message lastMessage { get; set; }
    }
}
