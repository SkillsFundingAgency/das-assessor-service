using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Azure;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Web.StartupConfiguration;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize(Policy = Policies.ExternalApiAccess)]
    public class ExternalApiController : Controller
    {
        private readonly ILogger<ExternalApiController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAzureApiClient _apiClient;

        public ExternalApiController(ILogger<ExternalApiController> logger, IHttpContextAccessor contextAccessor, IAzureApiClient apiClient)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ukprn = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.portal.com/ukprn")?.Value;
            var email = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.portal.com/mail")?.Value;

            var user = await _apiClient.GetUserDetailsByUkprn(ukprn, true) ?? await _apiClient.GetUserDetailsByEmail(email, true);
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> EnableAccess()
        {
            var ukprn = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var result = await _apiClient.CreateUser(ukprn, username);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAccess(AzureUser viewModel)
        {
            var ukprn = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            if (!string.Equals(viewModel.State, "blocked"))
            {
                await _apiClient.DeleteUser(viewModel.Id);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RenewPrimaryKey(string subscriptionId, string userId)
        {
            var user = await _apiClient.RegeneratePrimarySubscriptionKey(subscriptionId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RenewSecondaryKey(string subscriptionId, string userId)
        {
            var user = await _apiClient.RegenerateSecondarySubscriptionKey(subscriptionId);
            return RedirectToAction("Index");
        }
    }
}
