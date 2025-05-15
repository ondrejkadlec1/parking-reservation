using System.Text.Json.Serialization;
using ParkingReservation.Dtos.Reservations;

namespace ParkingReservation.Dtos.Interfaces
{
    [JsonDerivedType(typeof(ReservationResponseDto), typeDiscriminator: "normal")]
    [JsonDerivedType(typeof(BlockingResponseDto), typeDiscriminator: "blocking")]
    public interface IReservationDto
    {
    }
}
