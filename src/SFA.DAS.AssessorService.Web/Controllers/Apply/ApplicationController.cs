using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.ApplyService.Web.Controllers.Apply
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly ILogger<ApplicationController> _logger;
        private readonly IOrganisationsApiClient _apiClient;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IApplicationApiClient _applicationApiClient;

        public ApplicationController(IOrganisationsApiClient apiClient, 
            IContactsApiClient contactsApiClient, IApplicationApiClient applicationApiClient, ILogger<ApplicationController> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
            _contactsApiClient = contactsApiClient;
            _applicationApiClient = applicationApiClient;
        }

        [HttpGet("/Application")]
        public async Task<IActionResult> Applications()
        {
            var user = User.Identity.Name;
            var signinId = User.Claims.First(c => c.Type == "sub")?.Value;
            var contact = await _contactsApiClient.GetContactBySignInId(signinId);

            var userId = contact?.Id ?? Guid.Empty;

            _logger.LogInformation($"Got LoggedInUser from Session: {user}");

            var org = await _apiClient.GetOrganisationByUserId(userId);
            var applications = await _applicationApiClient.GetApplications(userId, false);
            applications = applications.Where(app => app.ApplicationStatus != ApplicationStatus.Rejected).ToList();

            if (!applications.Any())
            {
                //ON-2068 Registered org  with no application created via digital service then
                //display empty list of application screen
                if (org != null)
                    return org.RoEPAOApproved ? View(applications) : View("~/Views/Application/Declaration.cshtml");

            }
            //ON-2068 If there is an existing application for an org that is registered then display it
            //in a list of application screen
            if (applications.Count() == 1 && (org != null && org.RoEPAOApproved))
                return View(applications);

            if (applications.Count() > 1)
                return View(applications);

            //This always return one record otherwise the previous logic would have handled the response
            var application = applications.First();

            switch (application.ApplicationStatus)
            {
                case ApplicationStatus.FeedbackAdded:
                    return View("~/Views/Application/FeedbackIntro.cshtml", application.Id);
                case ApplicationStatus.Rejected:
                case ApplicationStatus.Approved:
                    return View(applications);
                default:
                    return RedirectToAction("SequenceSignPost", new { applicationId = application.Id });
            }

        }

        [HttpPost("/Application")]
        public async Task<IActionResult> StartApplication()
        {
            // var response = await _apiClient.StartApplication(await _userService.GetUserId());

            // return RedirectToAction("SequenceSignPost", new { applicationId = response.ApplicationId });

            return View();
        }

        [HttpGet("/Application/{applicationId}")]
        public async Task<IActionResult> SequenceSignPost(Guid applicationId)
        {
            return View();
        }
    }
}