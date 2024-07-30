using BuscoAPI.DTOS.Users;
using BuscoAPI.Entities;

namespace BuscoAPI.DTOS.Chat
{
    public class ChatDTO
    {
        public UserApplicationDTO user { get; set; }
        public Message lastMessage { get; set; }
    }
}
