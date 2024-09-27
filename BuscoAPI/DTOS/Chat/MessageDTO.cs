using BuscoAPI.DTOS.Users;
using BuscoAPI.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS.Chat
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public int UserIdSender { get; set; }
        public int UserIdReceiver { get; set; }
        public String Text { get; set; }
        public DateTime DateAndTime { get; set; }

        public UserBasicInfoDTO UserSender { get; set; }
    }
}
