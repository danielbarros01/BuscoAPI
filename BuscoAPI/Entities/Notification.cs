using BuscoAPI.DTOS.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        [Column("User_Receive_Id")]
        public int UserReceiveId { get; set; }
        [Column("User_Sender_Id")]
        public int UserSenderId { get; set; }
        [Column("Date_and_time")]
        public DateTime DateAndTime { get; set; } = DateTime.Now;
        public String Text { get; set; }
        [Column("Proposal_Id")]
        public int? ProposalId { get; set; }

        public User? UserSender { get; set; }
        public Proposal? Proposal { get; set; }
    }
}
