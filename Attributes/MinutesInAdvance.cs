using System.ComponentModel.DataAnnotations;
using Microsoft.OData.ModelBuilder;

namespace ParkingReservation.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MinutesInAdvance : ValidationAttribute
    {
        TimeSpan _inAdvance;
        public MinutesInAdvance(int minutes)
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
                return new ValidationResult($"Časový údaj musí být alespoň {_inAdvance.Minutes} minut v budoucnosti.");
            }
            return new ValidationResult("Wrong type.");
        }
    }
}
