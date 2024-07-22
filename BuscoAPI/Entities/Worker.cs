using BuscoAPI.DTOS.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BuscoAPI.Entities
{
    public class Worker
    {
        [Key]
        [Column("User_id")]
        public int UserId { get; set; }
        public String Title { get; set; }
        [Column("Years_of_experience")]
        public int YearsExperience { get; set; }
        [Column("Web_page")]
        public String? WebPage { get; set; }
        public String Description { get; set; }

        public User? User { get; set; }

        public List<WorkersProfessions> WorkersProfessions { get; set; }

    }
}
