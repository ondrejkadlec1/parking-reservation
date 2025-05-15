using ParkingReservation.Dtos.Interfaces;

namespace ParkingReservation.Dtos.Reservations
{
    public record ReservationResponseDto : IReservationDto
    {
        public Guid Id { get; set; }
        public DateTime BeginsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DisplayName { get; set; } = null!;
        public string State { get; set; } = null!;
        public int SpaceNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
