using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AssessorService.Web.Orchestrators.Login
{
    public class LoginOrchestrator : ILoginOrchestrator
    {
        private readonly ILogger<LoginOrchestrator> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILoginApiClient _loginApiClient;

        public LoginOrchestrator(ILogger<LoginOrchestrator> logger, IHttpContextAccessor contextAccessor, ILoginApiClient loginApiClient)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _loginApiClient = loginApiClient;
        }
        public async Task<LoginResponse> Login()
        {
            var claims = _contextAccessor.HttpContext.User.Claims;
            foreach (var claim in claims)
            {
                _logger.LogInformation($"Claim received: {claim.Type} Value: {claim.Value}");
            }

            _logger.LogInformation("Start of PostSignIn");

            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var email = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/mail")?.Value;
            var displayName = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/displayname")?.Value;

            var roles = _contextAccessor.HttpContext.User.Claims.Where(c => c.Type == "http://schemas.portal.com/service")
                .Select(c => c.Value).ToList();

            var loginResult = await _loginApiClient.Login(new LoginRequest()
            {
                DisplayName = displayName,
                Email = email,
                UkPrn = int.Parse(ukprn),
                Username = username,
                Roles = roles
            });

            return loginResult;
        }
    }
}