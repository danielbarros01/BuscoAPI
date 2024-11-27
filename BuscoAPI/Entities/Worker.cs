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

        public List<Qualification>? Qualifications { get; set; }

        public float AverageQualification
        {
            get
            {
                if (Qualifications == null || !Qualifications.Any())
                {
                    return 0;
                }

                return (float)Qualifications.Average(q => q.Score);
            }
        }

        public int? NumberOfQualifications
        {
            get => Qualifications != null ? Qualifications.Count() : null;
        }

    }
}
