using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Web.Staff.Domain;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Applications;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Apply
{
    [Authorize(Roles = Roles.AssessmentDeliveryTeam + "," + Roles.CertificationTeam)]
    public class ApplicationController : Controller
    {
        private readonly ApplyApiClient _applyApiClient;

        public ApplicationController(ApplyApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
        }

        [HttpGet("/Applications/Open")]
        public async Task<IActionResult> OpenApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetOpenApplications();

            var paginatedApplications = new PaginatedList<ApplicationSummaryItem>(applications, applications.Count(), page, int.MaxValue);

            var viewmodel = new DashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Applications/OpenApplications.cshtml", viewmodel);
        }

        [HttpGet("/Applications/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetClosedApplications();

            var paginatedApplications = new PaginatedList<ApplicationSummaryItem>(applications, applications.Count(), page, int.MaxValue);

            var viewmodel = new DashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Applications/ClosedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Applications/{applicationId}")]
        public async Task<IActionResult> Application(Guid applicationId)
        {
            var activeSequence = await _applyApiClient.GetActiveSequence(applicationId);

            if (activeSequence.Status == "Submitted")
            {
                await _applyApiClient.StartApplicationReview(applicationId, activeSequence.SequenceId);
            }

            return View("~/Views/Apply/Applications/Sequence.cshtml", activeSequence);
        }

        [HttpGet("/Applications/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}")]
        public async Task<IActionResult> Section(Guid applicationId, int sequenceId, int sectionId)
        {
            var section = await _applyApiClient.GetSection(applicationId, sequenceId, sectionId);
            var sectionVm = new ApplicationSectionViewModel(section);
            return View("~/Views/Apply/Applications/Section.cshtml", sectionVm);
        }

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}")]
        public async Task<IActionResult> EvaluateSection(Guid applicationId, int sequenceId, int sectionId, bool? isSectionComplete)
        {
            var errorMessages = new Dictionary<string, string>();

            if(!isSectionComplete.HasValue)
            {
                errorMessages["IsSectionComplete"] = "Please state if this section is completed";
            }

            if (errorMessages.Any())
            {
                foreach(var error in errorMessages)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }               

                var section = await _applyApiClient.GetSection(applicationId, sequenceId, sectionId);
                var sectionVm = new ApplicationSectionViewModel(section);
                return View("~/Views/Apply/Applications/Section.cshtml", sectionVm);
            }

            await _applyApiClient.EvaluateSection(applicationId, sequenceId, sectionId, isSectionComplete.Value);

            return RedirectToAction("Application", new { applicationId });
        }

        [HttpGet("/Applications/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}")]
        public async Task<IActionResult> Page(Guid applicationId, int sequenceId, int sectionId, string pageId)
        {
            var page = await _applyApiClient.GetPage(applicationId, sequenceId, sectionId, pageId);
            var pageVm = new PageViewModel(page);

            return View("~/Views/Apply/Applications/Page.cshtml", pageVm);
        }

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}")]
        public async Task<IActionResult> Feedback(Guid applicationId, int sequenceId, int sectionId, string pageId, string feedbackMessage)
        {
            var errorMessages = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(feedbackMessage))
            {
                errorMessages["FeedbackMessage"] = "Please enter a feedback comment";
            }

            if (errorMessages.Any())
            {
                foreach (var error in errorMessages)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }

                var page = await _applyApiClient.GetPage(applicationId, sequenceId, sectionId, pageId);
                var pageVm = new PageViewModel(page);
                return View("~/Views/Apply/Applications/Page.cshtml", pageVm);
            }

            Feedback feedback = new Feedback { Message = feedbackMessage, From = "Staff member", Date = DateTime.UtcNow, IsNew = true };

            await _applyApiClient.AddFeedback(applicationId, sequenceId, sectionId, pageId, feedback);

            return RedirectToAction("Section", new {applicationId, sequenceId, sectionId});
        }

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}/DeleteFeedback")]
        public async Task<IActionResult> DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId)
        {
            await _applyApiClient.DeleteFeedback(applicationId, sequenceId, sectionId, pageId, feedbackId);

            return RedirectToAction("Page", new { applicationId, sequenceId, sectionId, pageId });
        }

        [HttpGet("/Applications/{applicationId}/Sequence/{sequenceId}/Assessment")]
        public async Task<IActionResult> Assessment(Guid applicationId, int sequenceId)
        {
            var activeSequence = await _applyApiClient.GetActiveSequence(applicationId);

            if (activeSequence is null || activeSequence.SequenceId != sequenceId || activeSequence.Sections.Any(s => s.Status != ApplicationSectionStatus.Evaluated))
            {
                // This is to stop the wrong sequence being approved or if not all sections are Evaluated
                return RedirectToAction("OpenApplications");
            }

            var viewModel = new ApplicationSequenceAssessmentViewModel(activeSequence);
            return View("~/Views/Apply/Applications/Assessment.cshtml", viewModel);
        }

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceId}/Return")]
        public async Task<IActionResult> Return(Guid applicationId, int sequenceId, string returnType)
        {
            var errorMessages = new Dictionary<string, string>();

            if(string.IsNullOrWhiteSpace(returnType))
            {
                errorMessages["ReturnType"] = "Please state what you would like to do next";
            }

            if (errorMessages.Any())
            {
                foreach (var error in errorMessages)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }

                var activeSequence = await _applyApiClient.GetActiveSequence(applicationId);
                var viewModel = new ApplicationSequenceAssessmentViewModel(activeSequence);
                return View("~/Views/Apply/Applications/Assessment.cshtml", viewModel);
            }

            await _applyApiClient.ReturnApplication(applicationId, sequenceId, returnType);

            return RedirectToAction("Returned", new { applicationId });
        }

        [HttpGet("/Applications/Returned")]
        public IActionResult Returned(Guid applicationId)
        {
            return View("~/Views/Apply/Applications/Returned.cshtml");
        }
        
        [HttpGet("Application/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}/Question/{questionId}/{filename}/Download")]
        
        //[HttpGet("/Application/{applicationId}/Page/{pageId}/Question/{questionId}/File/{filename}/Download")]
        public async Task<IActionResult> Download(Guid applicationId, int sequenceId, int sectionId, string pageId, string questionId, string filename)
        {
            var userId = Guid.NewGuid();

            var fileInfo = await _applyApiClient.FileInfo(applicationId, userId, sequenceId, sectionId, pageId, questionId, filename);
            
            var file = await _applyApiClient.Download(applicationId, userId, sequenceId,sectionId, pageId, questionId, filename);

            var fileStream = await file.Content.ReadAsStreamAsync();
            
            return File(fileStream, fileInfo.ContentType, fileInfo.Filename);
        }
    }
}