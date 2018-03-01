using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class TokenService : ITokenService
    {
        private readonly IWebConfiguration _configuration;

        public TokenService(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken()
        {
            var tenant = _configuration.ClientApiAuthentication.Domain;// 
            var clientId = _configuration.ClientApiAuthentication.ClientId;// 
            var appKey = _configuration.ClientApiAuthentication.ClientSecret;// 
            var resourceId = _configuration.ClientApiAuthentication.ResourceId;// 

            var authority = $"https://login.microsoftonline.com/{tenant}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(resourceId, clientCredential).Result;

            return result.AccessToken;
        }
    }
}