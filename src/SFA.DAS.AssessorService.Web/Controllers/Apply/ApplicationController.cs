using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.ApplyService.Web.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly ILogger<ApplicationController> _logger;
        private readonly IOrganisationsApiClient _apiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public ApplicationController(IHttpContextAccessor contextAccessor, IOrganisationsApiClient apiClient, ILogger<ApplicationController> logger)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _apiClient = apiClient;
        }

        [HttpGet("/Apply/Application")]
        public async Task<IActionResult> Applications()
        {
            var user = User.Identity.Name;

            _logger.LogInformation($"Got LoggedInUser from Session: {user}");

            return View("~/Views/Apply/Application/Declaration.cshtml");
        }

        [HttpPost("/Apply/Application")]
        public async Task<IActionResult> StartApplication()
        {
            // var response = await _apiClient.StartApplication(await _userService.GetUserId());

            // return RedirectToAction("SequenceSignPost", new { applicationId = response.ApplicationId });

            return View();
        }

    }
}