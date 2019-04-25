using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Azure;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels.ExternalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize(Policy = Policies.ExternalApiAccess)]
    public class ExternalApiController : Controller
    {
        private readonly ILogger<ExternalApiController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebConfiguration _webConfiguration;
        private readonly IAzureApiClient _apiClient;

        public ExternalApiController(ILogger<ExternalApiController> logger, IHttpContextAccessor contextAccessor, IWebConfiguration webConfiguration, IAzureApiClient apiClient)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _webConfiguration = webConfiguration;
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ukprn = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.portal.com/ukprn")?.Value;
            var email = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.portal.com/mail")?.Value;
            var productId = _webConfiguration.AzureApiAuthentication.ProductId;

            var users = await GetAllUsers(ukprn, email);
            var loggedInUser = users.Where(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            var subscriptionsToShow = users.SelectMany(u => u.Subscriptions.Where(s => s.IsActive && s.ProductId == productId)).ToList();
            var primaryContacts = subscriptionsToShow.SelectMany(s => users.Where(u => u.Id == s.UserId)).ToList();

            // For now we show all subscriptions from the logged in user and the 'productId' subscription if the primary contact has it too.
            if (loggedInUser != null)
            {
                subscriptionsToShow.AddRange(loggedInUser.Subscriptions.Where(s => s.IsActive));
            }

            var viewmodel = new ExternalApiDetailsViewModel
            {
                LoggedInUser = loggedInUser,
                PrimaryContacts = primaryContacts,
                SubscriptionsToShow = subscriptionsToShow.GroupBy(s => s.Id).Select(s => s.First())
            };

            return View(viewmodel);
        }

        private async Task<IEnumerable<AzureUser>> GetAllUsers(string ukprn, string email)
        {
            var users = new List<AzureUser>();

            var ukprnUsers = await _apiClient.GetUserDetailsByUkprn(ukprn, true);

            if (ukprnUsers != null)
            {
                users.AddRange(ukprnUsers);
            }

            var emailuser = await _apiClient.GetUserDetailsByEmail(email, true);

            if (emailuser != null && !users.Any(u => u.AzureId == emailuser.AzureId))
            {
                users.Add(emailuser);
            }

            return users;
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

            if (viewModel.IsActive)
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
