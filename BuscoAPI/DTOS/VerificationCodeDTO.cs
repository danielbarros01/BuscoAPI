using System.ComponentModel.DataAnnotations;

namespace BuscoAPI.DTOS
{
    public class VerificationCodeDTO
    {
        [Required]
        public int Code { get; set; }
    }
}
