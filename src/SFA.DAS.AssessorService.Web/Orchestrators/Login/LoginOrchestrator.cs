using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Infrastructure;

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
                OrganisationName = string.Empty,
                Result = LoginResult.ContactDoesNotExist
            };

            var user = _contextAccessor.HttpContext.User;
            if (user == null)
                return loginResult;

            foreach (var claim in user.Claims)
            {
                _logger.LogInformation($"Claim received: {claim.Type} Value: {claim.Value}");
            }

            _logger.LogInformation("Start of PostSignIn");

            var signinId = _contextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var email = _contextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var displayName = _contextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "display_name")?.Value;

            if (signinId != null)
            {
                loginResult = await _loginApiClient.Login(new LoginRequest()
                {
                    DisplayName = displayName,
                    Email = email,
                    SignInId = Guid.Parse(signinId)
                });
            }

            return loginResult;
        }
    }
}