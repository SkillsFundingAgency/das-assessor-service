using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
            var loginResult = new LoginResponse
            {
                Result = LoginResult.ContactDoesNotExist
            };

            var user = _contextAccessor.HttpContext.User;
            if (user == null)
                return loginResult;

            _logger.LogInformation("Start of PostSignIn");

            var govIdentifier = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = _contextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? _contextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "emailaddress")?.Value;
            var displayName = _contextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "display_name")?.Value;

            if (govIdentifier != null)
            {
                loginResult = await _loginApiClient.Login(new LoginRequest()
                {
                    DisplayName = displayName,
                    Email = email,
                    GovUkIdentifier = govIdentifier
                });
            }

            return loginResult;
        }
    }
}