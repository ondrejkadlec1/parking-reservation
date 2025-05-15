using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using ParkingReservation.Data;
using ParkingReservation.Services.Helpers;

namespace ParkingReservation.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly AppDbContext _context;
        public UserService(GraphClientHelper helper, AppDbContext context)
        {
            _graphClient = helper.Client;
            _context = context;
        }

        public async Task Synchronize()
        {
            var usersInCloud = await GetUsersInCloud();

            await using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var usersInDb = await _context.Users.ToListAsync();

                var deletedUserIds = new List<Guid>();
                foreach (var user in usersInDb)
                {
                    var userInCloud = usersInCloud!.FirstOrDefault(p => Guid.Parse(p.Id!) == user.Id);
                    if (userInCloud == null)
                    {
                        user.IsActive = false;
                        deletedUserIds.Add(user.Id);
                        continue;
                    }
                    if (userInCloud.DisplayName != null)
                    {
                        user.DisplayName = userInCloud.DisplayName;
                    }
                    user.IsActive = true;
                    usersInCloud.Remove(userInCloud);
                }
                await _context.SaveChangesAsync();

                await AddNewUsers(usersInCloud);
                await DeleteDescendants(deletedUserIds);

                await transaction.CommitAsync();
            }
        }

        private async Task<List<Microsoft.Graph.Models.User>> GetUsersInCloud()
        {
            var graphResponse = await _graphClient.Users.GetAsync(config =>
                config.QueryParameters.Select = ["id", "displayName"]);
            if (graphResponse == null)
            {
                throw new Exception("Problem with Microsoft Graph.");
            }
            return graphResponse.Value!;
        }

        private async Task AddNewUsers(List<Microsoft.Graph.Models.User> usersInCloud)
        {
            foreach (var user in usersInCloud)
            {
                var id = Guid.Parse(user.Id!);
                var newUser = new Models.User
                {
                    Id = id,
                    DisplayName = user.DisplayName ?? id.ToString(),
                    IsActive = true,
                    IsAdmin = false,
                };
                _context.Users.Add(newUser);
            }
            await _context.SaveChangesAsync();
        }

        private async Task DeleteDescendants(List<Guid> deletedUserIds)
        {
            await _context.Reservations
                .Where(p => deletedUserIds.Contains(p.UserId))
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(p => p.StateId, 3));
            await _context.SaveChangesAsync();
        }
    }
}
