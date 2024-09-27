using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS.Notification
{
    public class NotificationCreationDTO
    {
        public int UserReceiveId { get; set; }
        public String Text { get; set; }
        public int? ProposalId { get; set; }
    }
}
