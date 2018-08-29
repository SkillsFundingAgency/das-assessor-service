using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Models.Azure;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize]
    public class AzureController : Controller
    {
        private readonly ILogger<AzureController> _logger;
        private readonly AzureApiClient _apiClient;
        private readonly ISessionService _sessionService;

        public AzureController(ILogger<AzureController> logger, AzureApiClient apiClient, ISessionService sessionService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page = 1)
        {
            var users = await _apiClient.ListUsers(page.Value);
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(string userId)
        {
            var user = await _apiClient.GetUserDetails(userId, true);
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            CreateAzureUserViewModel viewModel = new CreateAzureUserViewModel
            {
                Products = await _apiClient.ListProducts()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateAzureUserViewModel viewModel)
        {
            if (await _apiClient.UserExists(viewModel.Email))
            {
                ModelState.AddModelError(nameof(viewModel.Email), "Email currently in use.");
            }

            if (!ModelState.IsValid)
            {
                viewModel.Products = await _apiClient.ListProducts();
                return View(viewModel);
            }

            var result = await _apiClient.CreateUser(viewModel.FirstName, viewModel.LastName, viewModel.Email, viewModel.UkPrn, viewModel.ProductId);
            return RedirectToAction("GetUser", new { userId = result.Id });
        }

        [HttpPost]
        public async Task<IActionResult> RenewPrimaryKey(string subscriptionId, string userId)
        {
            var user = await _apiClient.RegeneratePrimarySubscriptionKey(subscriptionId);
            return RedirectToAction("GetUser", new { userId });
        }

        [HttpPost]
        public async Task<IActionResult> RenewSecondaryKey(string subscriptionId, string userId)
        {
            var user = await _apiClient.RegenerateSecondarySubscriptionKey(subscriptionId);
            return RedirectToAction("GetUser", new { userId });
        }
    }
}