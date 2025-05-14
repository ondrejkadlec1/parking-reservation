using System.ComponentModel.DataAnnotations;

namespace ParkingReservation.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MinutesResolution : ValidationAttribute
    {
        private int _minutesResolution { get; }
        public MinutesResolution(int minutesResolution)
        {
            _minutesResolution = minutesResolution;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime)
            {
                var timestamp = (DateTime)value;
                if (timestamp.Second == 0 && timestamp.Minute % _minutesResolution == 0)
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"Čas rezervace má nesprávné rozlišení.");
            }
            return new ValidationResult("Wrong type.");
        }
    }
}
