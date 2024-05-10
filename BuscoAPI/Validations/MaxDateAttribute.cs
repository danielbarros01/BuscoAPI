using System.ComponentModel.DataAnnotations;

namespace BuscoAPI.Validations
{
    public class MaxDateAttribute : ValidationAttribute
    {
        public int YearsAgo { get; set; }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true; // La validación de nulo debería ser manejada por RequiredAttribute
            }

            var date = (DateTime)value;
            var maxDate = DateTime.Today.AddYears(YearsAgo);

            return date <= maxDate;
        }
    }
}
