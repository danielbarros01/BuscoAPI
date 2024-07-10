using BuscoAPI.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS.Proposals
{
    public class ProposalCreationDTO
    {
        [Required]
        [MaxLength(45)]
        public string Title { get; set; }

        [Required]
        [MinLength(20)]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MinLength(20)]
        [MaxLength(255)]
        public string Requirements { get; set; }

        [Required]
        public decimal MinBudget { get; set; }
        [Required]
        public decimal MaxBudget { get; set; }

        [Required]
        [FileSize(sizeMaxMb: 4)]
        [FileTypeValidation(grupoTipoArchivo: FileType.Image)]
        public IFormFile Image { get; set; }

        [Required]
        public int professionId { get; set; }

        public bool? Status { get; } = null;

        public DateTime Date { get; } = DateTime.Now;
    }
}
