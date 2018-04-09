using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Settings;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.InfrastructureServices
{
    public class TokenService 
    {
        private readonly IWebConfiguration _configuration;

        public TokenService(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken()
        {
            var tenantId = _configuration.ClientApiAuthentication.TenantId;// 
            var clientId = _configuration.ClientApiAuthentication.ClientId;// 
            var appKey = _configuration.ClientApiAuthentication.ClientSecret;// 
            var resourceId = _configuration.ClientApiAuthentication.ResourceId;// 

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(resourceId, clientCredential).Result;

            return result.AccessToken;
        }
    }
}