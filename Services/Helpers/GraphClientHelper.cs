using Azure.Identity;
using Microsoft.Graph;

namespace ParkingReservation.Services.Helpers
{
    public class GraphClientHelper
    {
        public GraphServiceClient Client { get; }
        public GraphClientHelper(IConfiguration config)
        {
            var scopes = config.GetSection("AzureAd:Scopes").Get<string[]>();
            var tenantId = config["AzureAd:TenantId"];
            var clientId = config["AzureAd:ClientId"];
            var clientSecret = config["AzureAd:ClientSecret"];
            var options = new DeviceCodeCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            Client = new GraphServiceClient(clientSecretCredential, scopes);
        }
    }
}
