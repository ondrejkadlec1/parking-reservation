using System.ComponentModel.DataAnnotations;
using Microsoft.OData.ModelBuilder;

namespace ParkingReservation.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TimeInAdvance : ValidationAttribute
    {
        TimeSpan _inAdvance;
        public TimeInAdvance(int minutes)
        {
            _inAdvance = TimeSpan.FromMinutes(minutes);
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime)
            {
                var min = DateTime.Now + _inAdvance;
                if ((DateTime)value >= min)
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"Rezervace musí trvat {_inAdvance.Minutes} minut předem.");
            }
            return new ValidationResult("Wrong type.");
        }
    }
}
