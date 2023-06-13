using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AssessorService.Settings;
using System;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class TokenService
        : IAssessorTokenService, IQnATokenService, IRoatpTokenService, IReferenceDataTokenService
    {
        private readonly IClientApiAuthentication _configuration;
        private readonly string _environmentName;

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
    }
}
