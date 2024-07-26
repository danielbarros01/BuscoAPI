using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS
{
    public class QualificationCreationDTO
    {
        [Required]
        [Range(1,5, ErrorMessage = "La puntuación debe estar entre 1 y 5")]
        public float Score { get; set; }

        [Required]
        [MaxLength(500)]
        public string Commentary { get; set; }

        [Required]
        public int WorkerUserId { get; set; }


        public DateTime Date {  get; set; } = DateTime.Now;
    }
}
