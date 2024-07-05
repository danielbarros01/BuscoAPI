using BuscoAPI.Entities;

namespace BuscoAPI.DTOS.Worker
{
    public class WorkersProfessionsDTO
    {
        public int WorkerId { get; set; }
        public int ProfessionId { get; set; }
        public Profession Profession { get; set; }
    }
}
