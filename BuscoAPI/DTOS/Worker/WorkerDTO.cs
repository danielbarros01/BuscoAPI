using BuscoAPI.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BuscoAPI.DTOS.Worker
{
    public class WorkerDTO
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public int YearsExperience { get; set; }
        public string WebPage { get; set; }
        public string Description { get; set; }
        public List<WorkersProfessions> WorkersProfessions { get; set; }
    }
}
