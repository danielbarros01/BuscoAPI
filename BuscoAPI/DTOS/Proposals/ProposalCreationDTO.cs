using BuscoAPI.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

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

        [Range(-90,90)]
        [Required]
        public double Latitude { get; set; }
        [Range(-180,180)]
        [Required]
        public double Longitude { get; set; }

        //[Required]
        [AllowNull]
        [FileSize(sizeMaxMb: 8)]
        [FileTypeValidation(grupoTipoArchivo: FileType.Image)]
        public IFormFile Image { get; set; }

        [Required]
        public int professionId { get; set; }

        public bool? Status { get; set; } = null;

        public DateTime Date { get; } = DateTime.Now;
    }
}
