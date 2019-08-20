using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class QnaTokenService : IQnaTokenService
    {
        private readonly IWebConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public QnaTokenService(IWebConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public string GetToken()
        {
            if (_hostingEnvironment.IsDevelopment())
                return string.Empty;

            var tenantId = _configuration.QnaApiAuthentication.TenantId;
            var clientId = _configuration.QnaApiAuthentication.ClientId;
            var appKey = _configuration.QnaApiAuthentication.ClientSecret;
            var resourceId = _configuration.QnaApiAuthentication.ResourceId;

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(resourceId, clientCredential).Result;

            return result.AccessToken;
        }
    }
}
