using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.Entities
{
    public class Message
    {
        public int Id { get; set; }
        [Column("User_id_sender")]
        public int UserIdSender { get; set; }
        [Column("User_id_receiver")]
        public int UserIdReceiver { get; set; }
        [Column("Message")]
        public String Text { get; set; }
        [Column("Date_and_Time")]
        public DateTime DateAndTime { get; set; }
    }
}
