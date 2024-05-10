using BuscoAPI.Validations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BuscoAPI.DTOS.Users
{
    public class UserImageDto
    {
        [Required]
        [FileSize(sizeMaxMb: 4)]
        [FileTypeValidation(grupoTipoArchivo: FileType.Image)]
        public IFormFile Image { get; set; }
    }
}
