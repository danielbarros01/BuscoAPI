using BuscoAPI.DTOS.Proposals;
using BuscoAPI.DTOS.Users;
using BuscoAPI.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS.Notification
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public int UserReceiveId { get; set; }
        public int UserSenderId { get; set; }
        public DateTime DateAndTime { get; set; } = DateTime.Now;
        public String Text { get; set; }
        public int? ProposalId { get; set; }

        public UserApplicationDTO? UserSender { get; set; }
        public ProposalDTO? Proposal { get; set; }
    }
}
