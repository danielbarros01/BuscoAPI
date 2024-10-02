using System.ComponentModel.DataAnnotations;

namespace BuscoAPI.Validations
{
    public class MinDateAttribute : ValidationAttribute
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public override bool IsValid(object value)
        {
            if(value == null) return true;

            var date = (DateTime)value;
            var minDate = new DateTime(Year, Month, Day);

            return date >= minDate;
        }
    }
}
