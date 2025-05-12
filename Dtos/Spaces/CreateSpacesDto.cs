using System.ComponentModel.DataAnnotations;

namespace ParkingReservation.Dtos.Spaces
{
    public record CreateSpacesDto
    {
        [Range(1, 9999)]
        public int Count { get; set; }
    }
}
