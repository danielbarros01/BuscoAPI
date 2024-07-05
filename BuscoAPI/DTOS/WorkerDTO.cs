using BuscoAPI.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BuscoAPI.DTOS
{
    public class WorkerDTO
    {
        public int UserId { get; set; }
        public String Title { get; set; }
        public int YearsExperience { get; set; }
        public String? WebPage { get; set; }
        public String Description { get; set; }
        public List<WorkersProfessions> WorkersProfessions { get; set; }
    }
}
