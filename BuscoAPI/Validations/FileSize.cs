using System.ComponentModel.DataAnnotations;

namespace BuscoAPI.Validations
{
    public class FileSize : ValidationAttribute
    {
        private readonly int sizeMaxMb;

        public FileSize(int sizeMaxMb)
        {
            this.sizeMaxMb = sizeMaxMb;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            //parse the data to formFile
            IFormFile formFile = value as IFormFile;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            //To convert pesoMaxMb to Bytes
            if (formFile.Length > sizeMaxMb * 1024 * 1024)
            {
                return new ValidationResult($"El peso del fichero no debe ser superior a {sizeMaxMb}mb");
            }

            //Sin errores
            return ValidationResult.Success;
        }
    }
}
