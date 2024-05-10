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

            //parsear el dato a formFile
            IFormFile formFile = value as IFormFile;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            //Si no se encuentra entre los tipos validos
            if (!validTypes.Contains(formFile.ContentType))
            {
                return new ValidationResult($"The file type must be one of the following:: {string.Join(", ", validTypes)}");
            }

            //Si no hay errores
            return ValidationResult.Success;
        }
    }
}
