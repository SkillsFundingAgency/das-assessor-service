using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AssessorService.Api.Common.Settings;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Api.Common
{
    public class TokenService : ITokenService
    {
        private readonly IClientConfiguration _clientConfiguration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IClientConfiguration clientConfiguration, ILogger<TokenService> logger)
        {
            _clientConfiguration = clientConfiguration;
            _logger = logger;
        }

        public async Task<string> GetTokenAsync()
        {
            try
            {
                if (_clientConfiguration is IAzureActiveDirectoryClientConfiguration azureActiveDirectoryClientConfiguration)
                {
                    if (IsLocal(azureActiveDirectoryClientConfiguration.ApiBaseAddress))
                        return string.Empty;

                    var authority = $"https://login.microsoftonline.com/{azureActiveDirectoryClientConfiguration.TenantId}";
                    var clientCredential = new ClientCredential(azureActiveDirectoryClientConfiguration.ClientId, azureActiveDirectoryClientConfiguration.ClientSecret);
                    var context = new AuthenticationContext(authority, true);
                    var result = await context.AcquireTokenAsync(azureActiveDirectoryClientConfiguration.ResourceId, clientCredential);

                    return result.AccessToken;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception when generating bearer token");
            }

            return string.Empty;
        }

        private bool IsLocal(string baseUrl)
        {
            Uri uri = new Uri(baseUrl);
            return (uri.Host == "localhost" || uri.Host == "127.0.0.1" || uri.Host == "::1");
        }
    }
}
