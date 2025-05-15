using System.Text.Json.Serialization;
using ParkingReservation.Dtos.Reservations;

namespace ParkingReservation.Dtos.Interfaces
{
    [JsonDerivedType(typeof(ReservationResponseDto), typeDiscriminator: "normal")]
    [JsonDerivedType(typeof(BlockingResponseDto), typeDiscriminator: "blocking")]
    public interface IReservationDto
    {
        public Guid Id { get; set; }
        public DateTime BeginsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DisplayName { get; set; }
        public int SpaceNumber { get; set; }
    }
}
