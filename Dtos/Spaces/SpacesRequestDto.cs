using System.ComponentModel.DataAnnotations;

namespace ParkingReservation.Dtos.Spaces
{
    public record SpacesRequestDto
    {
        [Range(1, 9999)]
        public int Count { get; set; }
    }
}
