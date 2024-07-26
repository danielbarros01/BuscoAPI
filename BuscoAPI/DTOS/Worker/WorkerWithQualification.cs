using BuscoAPI.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS.Worker
{
    public class WorkerWithQualification
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public int YearsExperience { get; set; }
        public string WebPage { get; set; }
        public string Description { get; set; }
        public List<WorkersProfessions> WorkersProfessions { get; set; }

        public User? User { get; set; }

        public int NumberOfQualifications { get; set; }
        public int AverageQualification { get; set; }
    }
}
