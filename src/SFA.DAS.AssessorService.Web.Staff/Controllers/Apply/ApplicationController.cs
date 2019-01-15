using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.ApplyTypes;
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

        [HttpGet("/Applications/Dashboard")]
        public IActionResult Dashboard()
        {
            return View("~/Views/Apply/Applications/Dashboard.cshtml");
        }

        [HttpGet("/Applications/New")]
        public async Task<IActionResult> Applications()
        {
            int sequenceId = 1;
            var applications = await _applyApiClient.NewApplications(sequenceId);

            return View("~/Views/Apply/Applications/Index.cshtml", applications);
        }

        [HttpGet("/Applications/Standards/New")]
        public async Task<IActionResult> Standards()
        {
            int sequenceId = 2;
            var applications = await _applyApiClient.NewApplications(sequenceId);

            return View("~/Views/Apply/Applications/Index.cshtml", applications);
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
        public async Task<IActionResult> EvaluateSection(Guid applicationId, int sequenceId, int sectionId, string feedbackMessage, bool addFeedbackMessage, bool? isSectionComplete)
        {
            var errorMessages = new Dictionary<string, string>();

            if (addFeedbackMessage && string.IsNullOrWhiteSpace(feedbackMessage))
            {
                errorMessages["FeedbackMessage"] = "Please enter a feedback comment";
            }

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

            Feedback feedback = null;

            if (addFeedbackMessage)
            {
                feedback = new Feedback { Message = feedbackMessage, From = "Staff member", Date = DateTime.UtcNow };
            }

            await _applyApiClient.EvaluateSection(applicationId, sequenceId, sectionId, feedback, isSectionComplete.Value);

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
            if(string.IsNullOrWhiteSpace(feedbackMessage))
            {
                string key = "FeedbackMessage";
                string errorMessage = "Please enter a feedback comment";
                ModelState.AddModelError(key, errorMessage);

                var page = await _applyApiClient.GetPage(applicationId, sequenceId, sectionId, pageId);
                var pageVm = new PageViewModel(page);

                return View("~/Views/Apply/Applications/Page.cshtml", pageVm);
            }

            await _applyApiClient.AddFeedback(applicationId, sequenceId, sectionId, pageId, feedbackMessage);

            return RedirectToAction("Page", new {applicationId, sequenceId, sectionId, pageId});
        }

        [HttpGet("/Applications/{applicationId}/Sequence/{sequenceId}/Assessment")]
        public async Task<IActionResult> Assessment(Guid applicationId, int sequenceId)
        {
            var activeSequence = await _applyApiClient.GetActiveSequence(applicationId);

            if (activeSequence is null || activeSequence.SequenceId != sequenceId || activeSequence.Sections.Any(s => s.Status != ApplicationSectionStatus.Evaluated))
            {
                // This is to stop the wrong sequence being approved or if not all sections are Evaluated
                return RedirectToAction("Applications");
            }

            var viewModel = new ApplicationSequenceAssessmentViewModel(activeSequence);
            return View("~/Views/Apply/Applications/Assessment.cshtml", viewModel);
        }

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceId}/Return")]
        public async Task<IActionResult> Return(Guid applicationId, int sequenceId, string returnType, bool addFeedbackMessage, string feedbackMessage, bool? approvewithcomment)
        {
            var errorMessages = new Dictionary<string, string>();

            if(string.IsNullOrWhiteSpace(returnType))
            {
                errorMessages["ReturnType"] = "Please state what you would like to do next";
            }
            else if(returnType == "Approve")
            {
                if (approvewithcomment == null)
                {
                    errorMessages["ReturnType"] = "Please state what you would like to do next";
                }
                else if (approvewithcomment == true && string.IsNullOrWhiteSpace(feedbackMessage))
                {
                    errorMessages["FeedbackMessage"] = "Please enter a feedback comment";
                }
            }
            else if(addFeedbackMessage == true && string.IsNullOrWhiteSpace(feedbackMessage))
            {
                errorMessages["FeedbackMessage"] = "Please enter a feedback comment";
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

            Feedback feedback = null;

            if (addFeedbackMessage)
            {
                feedback = new Feedback { Message = feedbackMessage, From = "Staff member", Date = DateTime.UtcNow };
            }

            await _applyApiClient.ReturnApplication(applicationId, sequenceId, feedback, returnType);

            return RedirectToAction("Returned", new { applicationId });
        }

        [HttpGet("/Applications/Returned")]
        public IActionResult Returned(Guid applicationId)
        {
            return View("~/Views/Apply/Applications/Returned.cshtml");
        }
    }
}