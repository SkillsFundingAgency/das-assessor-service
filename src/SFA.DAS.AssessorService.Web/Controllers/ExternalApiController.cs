using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Azure;
using SFA.DAS.AssessorService.Web.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    public class ExternalApiController : Controller
    {
        private readonly ILogger<ExternalApiController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAzureApiClient _apiClient;
        private readonly ISessionService _sessionService;

        public ExternalApiController(ILogger<ExternalApiController> logger, IHttpContextAccessor contextAccessor, IAzureApiClient apiClient, ISessionService sessionService)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _apiClient = apiClient;
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ukprn = _contextAccessor.HttpContext.User.Claims.First(c => c.Type == "http://schemas.portal.com/ukprn")?.Value;

            var user = await _apiClient.GetUserDetailsByUkprn(ukprn, true);
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> EnableAccess()
        {
            var ukprn = _contextAccessor.HttpContext.User.Claims.First(c => c.Type == "http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var result = await _apiClient.CreateUser(ukprn, username);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAccess(AzureUser viewModel)
        {
            var ukprn = _contextAccessor.HttpContext.User.Claims.First(c => c.Type == "http://schemas.portal.com/ukprn")?.Value;
            var username = _contextAccessor.HttpContext.User.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

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
