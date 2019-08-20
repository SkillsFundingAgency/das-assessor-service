using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.ApplyService.Web.Controllers.Apply
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly ILogger<ApplicationController> _logger;
        private readonly IOrganisationsApiClient _apiClient;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IApplicationApiClient _applicationApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private const string WorkflowType = "EPAO";

        public ApplicationController(IOrganisationsApiClient apiClient, IQnaApiClient qnaApiClient,
            IContactsApiClient contactsApiClient, IApplicationApiClient applicationApiClient, ILogger<ApplicationController> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
            _contactsApiClient = contactsApiClient;
            _applicationApiClient = applicationApiClient;
            _qnaApiClient = qnaApiClient;
        }

        [HttpGet("/Application")]
        public async Task<IActionResult> Applications()
        {
            _logger.LogInformation($"Got LoggedInUser from Session: { User.Identity.Name}");

            var userId = await GetUserId();
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
                    return RedirectToAction("SequenceSignPost", new { Id = application.Id });
            }

        }

        [HttpPost("/Application")]
        public async Task<IActionResult> StartApplication()
        {
            var userId = await GetUserId();
            var org = await _apiClient.GetOrganisationByUserId(userId);

            var applicationStartRequest = new StartApplicationRequest
            {
                UserReference = userId.ToString(),
                WorkflowType = WorkflowType,
                ApplicationData = JsonConvert.SerializeObject(new ApplicationData {
                    UseTradingName = false,
                    OrganisationName = org.EndPointAssessorName,
                    OrganisationReferenceId = org.Id.ToString()
                })
            };

            var qnaResponse = await _qnaApiClient.StartApplications(applicationStartRequest);

            //Todo: Create a page to go to if qna fails checks or invalid response is received
            var  appResponse = await _applicationApiClient.CreateApplication(new CreateApplicationRequest
            {
                ApplicationStatus = ApplicationStatus.InProgress,
                OrganisationId = org.Id,
                QnaApplicationId = qnaResponse?.ApplicationId??Guid.Empty,
                UserId = userId
            });

            return RedirectToAction("SequenceSignPost", new { Id = appResponse });
        }

        [HttpGet("/Application/{Id}")]
        public async Task<IActionResult> SequenceSignPost(Guid Id)
        {
            var userId = await GetUserId();
            var application = await _applicationApiClient.GetApplication(Id);

            if (application is null)
            {
                return RedirectToAction("Applications");
            }

            if (application.ApplicationStatus == ApplicationStatus.Approved)
            {
                return View("~/Views/Application/Approved.cshtml", application);
            }

            if (application.ApplicationStatus == ApplicationStatus.Rejected)
            {
                return View("~/Views/Application/Rejected.cshtml", application);
            }

            if (application.ApplicationStatus == ApplicationStatus.FeedbackAdded)
            {
                return View("~/Views/Application/FeedbackIntro.cshtml", application.Id);
            }

            var sequence = await _qnaApiClient.GetApplicationActiveSequence(application.ApplicationId);

            StandardApplicationData applicationData = null;

            if (application.ApplicationData != null)
            {
                applicationData = new StandardApplicationData
                {
                    StandardName = application.ApplicationData.StandardName
                };
            }
            var sequenceNo = (SequenceNo)sequence.SequenceNo;

            //// Only go to search if application hasn't got a selected standard?
            if (sequenceNo == SequenceNo.Stage1)
            {
                return RedirectToAction("Sequence", new { application.ApplicationId });
            }
            else if (sequenceNo == SequenceNo.Stage2 && string.IsNullOrWhiteSpace(applicationData?.StandardName))
            {
                var org = await _apiClient.GetOrganisationByUserId(userId);
                if (org.RoEPAOApproved)
                {
                   return RedirectToAction("Index", "Standard", new { application.Id });
                }

                return View("~/Views/Application/Stage2Intro.cshtml", application.Id);
            }
            else if (sequenceNo == SequenceNo.Stage2)
            {
                return RedirectToAction("Sequence", new { application.Id });
            }

            throw new BadRequestException("Section does not have a valid DisplayType");
        }

        [HttpGet("/Application/{Id}/Sequence")]
        public async Task<IActionResult> Sequence(Guid Id)
        {
            var application = await _applicationApiClient.GetApplication(Id);
            var sequence = await _qnaApiClient.GetApplicationActiveSequence(application.ApplicationId);
            var sections = await _qnaApiClient.GetSections(application.ApplicationId, sequence.Id);
            
            var sequenceVm = new SequenceViewModel(sequence, application.Id, sections, null);
            return View(sequenceVm);
        }

        private async Task<Guid> GetUserId()
        {
            var signinId = User.Claims.First(c => c.Type == "sub")?.Value;
            var contact = await _contactsApiClient.GetContactBySignInId(signinId);

            return contact?.Id ?? Guid.Empty;
        }
    }
}