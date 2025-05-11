using System.ComponentModel.DataAnnotations;
using ParkingReservation.Dtos.Reservations;

namespace ParkingReservation.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MinimalDuration: ValidationAttribute
    {
        TimeSpan _minimalDuration;
        public MinimalDuration(int minutes) 
        {
            _minimalDuration = TimeSpan.FromMinutes(minutes);
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is ReservationRequestDto)
            {
                var dto = (ReservationRequestDto)value;
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
