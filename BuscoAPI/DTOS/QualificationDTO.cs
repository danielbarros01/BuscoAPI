using BuscoAPI.DTOS.Users;
using BuscoAPI.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS
{
    public class QualificationDTO
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string Commentary { get; set; }
        public DateTime Date { get; set; }

        [Column("User_id")]
        public int UserId { get; set; }

        [Column("Worker_user_id")]
        public int WorkerUserId { get; set; }

        public UserBasicDTO? User { get; set; }
    }
}
