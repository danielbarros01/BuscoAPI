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

            IFormFile formFile = value as IFormFile;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            if (formFile.Length > sizeMaxMb * 1024 * 1024)
            {
                return new ValidationResult($"El peso del fichero no debe ser superior a {sizeMaxMb}mb");
            }

            return ValidationResult.Success;
        }
    }
}
