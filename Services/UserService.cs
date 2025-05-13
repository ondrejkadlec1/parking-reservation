using ParkingReservation.Services.Interfaces;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using ParkingReservation.Services.Results;

namespace ParkingReservation.Services
{
    public class UserService(GraphClientHelper helper) : IUserService
    {
        GraphServiceClient _graphClient = helper.Client;
        
        public async Task<ServiceCallResult<Dictionary<string, string>>> GetUsernames(ICollection<string> ids)
        {
            var batchRequestContent = new BatchRequestContentCollection(_graphClient);

            var userIdToRequestId = new Dictionary<string, string>();
            foreach (var id in ids)
            {
                var userRequest = _graphClient.Users[id].ToGetRequestInformation(config =>
                    config.QueryParameters.Select = ["displayName"]);
                var userRequestId = await batchRequestContent.AddBatchRequestStepAsync(userRequest);
                userIdToRequestId.Add(id, userRequestId);
            }
            var returnedResponse = await _graphClient.Batch.PostAsync(batchRequestContent);
            
            var userIdToUsername = new Dictionary<string, string>();
            foreach(KeyValuePair<string, string> pair in userIdToRequestId)
            {
                var user = await returnedResponse.GetResponseByIdAsync<User>(pair.Value);
                var username = user.DisplayName != null ? user.DisplayName : "unknown"; 
                userIdToUsername.Add(pair.Key, username);
            }
            return new ServiceCallResult<Dictionary<string, string>> { 
                Object = userIdToUsername,
                Success = true
            };
        }
    }
}
