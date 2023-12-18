using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class TokenService : IAssessorTokenService, IQnATokenService, IRoatpTokenService, IReferenceDataTokenService
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
                else if (_clientConfiguration is IManagedIdentityClientConfiguration managedIdentityClientConfiguration)
                {
                    if (IsLocal(managedIdentityClientConfiguration.ApiBaseUrl))
                        return string.Empty;

                    var defaultAzureCredential = new DefaultAzureCredential( new DefaultAzureCredentialOptions());
                    var result = await defaultAzureCredential.GetTokenAsync(
                        new TokenRequestContext(scopes: new string[] { managedIdentityClientConfiguration.IdentifierUri + "/.default" }) { });

                    return result.Token;
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
