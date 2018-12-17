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
            var applications = await _applyApiClient.ReviewApplications();

            return View("~/Views/Apply/Applications/Index.cshtml", applications);
        }

        [HttpGet("/Applications/{applicationId}")]
        public async Task<IActionResult> Application(Guid applicationId)
        {
            var activeSequence = await _applyApiClient.GetActiveSequence(applicationId);
            activeSequence.Sections = activeSequence.Sections.Where(s => s.SectionId != 3).ToList();
            return View("~/Views/Apply/Applications/Sequence.cshtml", activeSequence);
        }
        
        [HttpGet("/Applications/{applicationId}/Sequence/{sequenceId}/Section/{sectionId}")]
        public async Task<IActionResult> Section(Guid applicationId, int sequenceId, int sectionId)
        {
            var section = await _applyApiClient.GetSection(applicationId, sequenceId, sectionId);
            
            return View("~/Views/Apply/Applications/Section.cshtml", section);
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

        [HttpPost("/Applications/{applicationId}/Sequence/{sequenceId}/Return")]
        public async Task<IActionResult> Return(Guid applicationId, int sequenceId, string returnType)
        {
            await _applyApiClient.ReturnApplication(applicationId, sequenceId, returnType);

            return RedirectToAction("Applications");
        }
    }
}