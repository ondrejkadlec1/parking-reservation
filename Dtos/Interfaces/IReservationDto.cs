using System.Text.Json.Serialization;
using ParkingReservation.Dtos.Reservations;

namespace ParkingReservation.Dtos.Interfaces
{
    [JsonDerivedType(typeof(ReservationDto), typeDiscriminator: "normal")]
    [JsonDerivedType(typeof(BlockingDto), typeDiscriminator: "blocking")]
    public interface IReservationDto
    {
    }
}
