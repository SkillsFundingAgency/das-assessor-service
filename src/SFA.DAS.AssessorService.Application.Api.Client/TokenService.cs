using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class TokenService : ITokenService
    {
        private readonly IClientApiAuthentication _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public TokenService(IClientApiAuthentication configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public string GetToken()
        {
            if (_hostEnvironment.IsDevelopment())
                return string.Empty;

            var tenantId = _configuration.TenantId;
            var clientId = _configuration.ClientId;
            var appKey = _configuration.ClientSecret;
            var resourceId = _configuration.ResourceId;

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(resourceId, clientCredential).GetAwaiter().GetResult();

            return result?.AccessToken;
        }
    }
}
