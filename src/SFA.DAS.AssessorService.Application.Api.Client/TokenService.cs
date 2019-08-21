using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class TokenService : ITokenService
    {
        private readonly IWebConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly bool _useSandbox;

        public TokenService(IWebConfiguration configuration, IHostingEnvironment hostingEnvironment, bool useSandbox)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _useSandbox = useSandbox;
        }

        public string GetToken()
        {
            string token;

            if (_hostingEnvironment.IsDevelopment())
            {
                token = string.Empty;
            }
            else
            {
                var tenantId = _useSandbox ? _configuration.SandboxClientApiAuthentication.TenantId : _configuration.ClientApiAuthentication.TenantId;
                var clientId = _useSandbox ? _configuration.SandboxClientApiAuthentication.ClientId : _configuration.ClientApiAuthentication.ClientId;
                var appKey = _useSandbox ? _configuration.SandboxClientApiAuthentication.ClientSecret : _configuration.ClientApiAuthentication.ClientSecret;
                var resourceId = _useSandbox ? _configuration.SandboxClientApiAuthentication.ResourceId : _configuration.ClientApiAuthentication.ResourceId;

                var authority = $"https://login.microsoftonline.com/{tenantId}";
                var clientCredential = new ClientCredential(clientId, appKey);
                var context = new AuthenticationContext(authority, true);
                var result = context.AcquireTokenAsync(resourceId, clientCredential).GetAwaiter().GetResult();

                token = result?.AccessToken;
            }
                
            return token;
        }
    }
}