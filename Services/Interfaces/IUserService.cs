using ParkingReservation.Services.Results;


namespace ParkingReservation.Services.Interfaces
{
    public interface IUserService
    {
        public Task<ServiceCallResult<Dictionary<string, string>>> GetUsernames(ICollection<string> ids);
    }
}
