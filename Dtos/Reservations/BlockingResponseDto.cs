using ParkingReservation.Dtos.Interfaces;

namespace ParkingReservation.Dtos.Reservations
{
    public record BlockingResponseDto : IReservationDto, IOwnedDto
    {
        public Guid Id { get; set; }
        public DateTime BeginsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DisplayName { get; set; } = null!;
        public int SpaceNumber { get; set; }
        public string? Comment { get; set; }
    }
}
