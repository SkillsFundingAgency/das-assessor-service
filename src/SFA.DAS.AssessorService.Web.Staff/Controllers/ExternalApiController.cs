using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Azure;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize(Policy = Startup.Policies.OperationsTeamOnly)]
    public class ExternalApiController : Controller
    {
        private readonly ILogger<ExternalApiController> _logger;
        private readonly IAzureApiClient _apiClient;

        public ExternalApiController(ILogger<ExternalApiController> logger, IAzureApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page = 1)
        {
            var users = await _apiClient.ListUsers(page.Value);
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(string userId)
        {
            var user = await _apiClient.GetUserDetails(userId, true);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EnableUser(string userId)
        {
            await _apiClient.EnableUser(userId);
            return RedirectToAction("UserDetails", new { userId });
        }

        [HttpPost]
        public async Task<IActionResult> DisableUser(string userId)
        {
            await _apiClient.DisableUser(userId);
            return RedirectToAction("UserDetails", new { userId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _apiClient.DeleteUser(userId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RenewPrimaryKey(string subscriptionId, string userId)
        {
            var user = await _apiClient.RegeneratePrimarySubscriptionKey(subscriptionId);
            return RedirectToAction("UserDetails", new { userId });
        }

        [HttpPost]
        public async Task<IActionResult> RenewSecondaryKey(string subscriptionId, string userId)
        {
            var user = await _apiClient.RegenerateSecondarySubscriptionKey(subscriptionId);
            return RedirectToAction("UserDetails", new { userId });
        }
    }
}