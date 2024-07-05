using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.Entities
{
    public class WorkersProfessions
    {
        [Column("Worker_user_id")]
        public int WorkerId { get; set; }
        [Column("Profession_id")]
        public int ProfessionId { get; set; }

        //public Worker Worker { get; set; }
        public Profession Profession { get; set; }
    }
}
