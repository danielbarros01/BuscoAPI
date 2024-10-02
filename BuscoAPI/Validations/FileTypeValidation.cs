using System.ComponentModel.DataAnnotations;

namespace BuscoAPI.Validations
{
    public class FileTypeValidation : ValidationAttribute
    {
        private readonly string[] validTypes;

        public FileTypeValidation(string[] tiposValidos)
        {
            this.validTypes = tiposValidos;
        }

        public FileTypeValidation(FileType grupoTipoArchivo)
        {
            if (grupoTipoArchivo == FileType.Image)
            {
                validTypes = new string[] { "image/jpeg", "image/png", "image/jpg" };
            }
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

            if (!validTypes.Contains(formFile.ContentType))
            {
                return new ValidationResult($"El tipo de archivo debe ser uno de los siguientes: {string.Join(", ", validTypes)}");
            }

            return ValidationResult.Success;
        }
    }
}
