using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Web.Staff.Domain;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;

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

            return View("~/Views/Apply/Applications/Section.cshtml", section);
        }

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}")]
        public async Task<IActionResult> CompleteSection(Guid applicationId, int sequenceId, int sectionId, string feedbackComment, bool applyFeedbackComment, bool isSectionComplete)
        {
            if (!applyFeedbackComment) feedbackComment = null;

            await _applyApiClient.CompleteSection(applicationId, sequenceId, sectionId, feedbackComment, isSectionComplete);

            return RedirectToAction("Application", new { applicationId });
        }

        [HttpGet("/Applications/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}")]
        public async Task<IActionResult> Page(Guid applicationId, int sequenceId, int sectionId, string pageId)
        {
            var page = await _applyApiClient.GetPage(applicationId, sequenceId, sectionId, pageId);

            page.ApplicationId = applicationId;

            return View("~/Views/Apply/Applications/Page.cshtml", page);
        }

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}")]
        public async Task<IActionResult> Feedback(Guid applicationId, int sequenceId, int sectionId, string pageId, string message)
        {
            await _applyApiClient.AddFeedback(applicationId, sequenceId, sectionId, pageId, message);

            return RedirectToAction("Page", new {applicationId, sequenceId, sectionId, pageId});
        }

        [HttpGet("/Applications/{applicationId}/Sequence/{sequenceId}/Assessment")]
        public async Task<IActionResult> Assessment(Guid applicationId, int sequenceId)
        {
            var activeSequence = await _applyApiClient.GetActiveSequence(applicationId);

            if (activeSequence is null || activeSequence.SequenceId != sequenceId || activeSequence.Sections.Any(s => s.Status != "Completed"))
            {
                // This is to stop the wrong sequence being approved or if not all sections are completed
                return RedirectToAction("Applications");
            }

            return View("~/Views/Apply/Applications/Assessment.cshtml", activeSequence);
        }

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceId}/Return")]
        public async Task<IActionResult> Return(Guid applicationId, int sequenceId, string returnType)
        {
            await _applyApiClient.ReturnApplication(applicationId, sequenceId, returnType);

            return RedirectToAction("Applications");
        }
    }
}