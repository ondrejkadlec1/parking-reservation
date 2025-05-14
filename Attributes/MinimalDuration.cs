using System.ComponentModel.DataAnnotations;
using ParkingReservation.Dtos.Interfaces;

namespace ParkingReservation.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class MinimalDuration : ValidationAttribute
    {
        private TimeSpan _minimalDuration { get; set; }
        public MinimalDuration(int minutes)
        {
            _minimalDuration = TimeSpan.FromMinutes(minutes);
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is ITimeInterval)
            {
                var dto = (ITimeInterval)value;
                if (dto.EndsAt - dto.BeginsAt >= _minimalDuration)
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"Rezervace musí trvat {_minimalDuration.Minutes} minut předem.");
            }
            return new ValidationResult("Wrong type.");

        }
    }
}
