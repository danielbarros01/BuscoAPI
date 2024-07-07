using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace BuscoAPI.Validations
{
    public class AtLeastOneElementAttribute : ValidationAttribute
    {
        public AtLeastOneElementAttribute(string errorMessage) : base(errorMessage) { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var list = value as IList;

            if (list != null && list.Count > 0) { 
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
