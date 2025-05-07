using ParkingReservation.Dtos.Interfaces;

namespace ParkingReservation.Dtos
{
    public record SpaceDetailDto
    {
        public int SpaceNumber { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public ICollection<ReservationDto> Reservations { get; set; }
    }
}
