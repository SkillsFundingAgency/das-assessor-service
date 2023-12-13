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
        private readonly IClientApiAuthentication _configuration;
        private readonly IManagedIdentityApiAuthentication _apiAuthentication;
        private readonly ILogger<TokenService> _logger;
        private readonly string _environmentName;

        public TokenService(IClientApiAuthentication apiAuthentication)
        {
            _apiAuthentication = apiAuthentication;
        }

        public TokenService(IManagedIdentityApiAuthentication apiAuthentication, ILogger<TokenService> logger)
        {
            _apiAuthentication = apiAuthentication;
            _logger = logger;
        }

        public TokenService(IClientApiAuthentication configuration, string environmentName)
        {
            _configuration = configuration;
            _environmentName = environmentName;
        }

        public string GetToken()
        {
            Uri uri = new Uri(_configuration.ApiBaseAddress);
            if (uri.Host == "localhost" || uri.Host == "127.0.0.1" || uri.Host == "::1")
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

        public async Task<string> GetTokenAsync()
        {
            try
            {
                Uri uri = new Uri(_apiAuthentication.ApiBaseUrl);
                if (uri.Host == "localhost" || uri.Host == "127.0.0.1" || uri.Host == "::1")
                    return string.Empty;

                if (_apiAuthentication is IClientApiAuthentication clientApiAuthentication)
                {
                    var authority = $"{clientApiAuthentication.Instance}/{clientApiAuthentication.TenantId}";
                    var clientCredential = new ClientCredential(clientApiAuthentication.ClientId, clientApiAuthentication.ClientSecret);
                    var context = new AuthenticationContext(authority, true);
                    var result = await context.AcquireTokenAsync(clientApiAuthentication.IdentifierUri, clientCredential);

                    return result.AccessToken;
                }
                else if (_apiAuthentication is IManagedIdentityApiAuthentication managedIdentityApiAuthenication)
                {
   
                    var defaultAzureCredential = new DefaultAzureCredential( new DefaultAzureCredentialOptions());

                    _logger.LogInformation($"Getting token at {DateTime.UtcNow}");

                    var result = await defaultAzureCredential.GetTokenAsync(
                        new TokenRequestContext(scopes: new string[] { managedIdentityApiAuthenication.IdentifierUri + "/.default" }) { });

                    _logger.LogInformation($"Received token at {DateTime.UtcNow}");

                    return result.Token;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"Exception getting token at {DateTime.UtcNow}  ex: {ex.Message}");
            }
            

            return string.Empty;
        }
    }
}
