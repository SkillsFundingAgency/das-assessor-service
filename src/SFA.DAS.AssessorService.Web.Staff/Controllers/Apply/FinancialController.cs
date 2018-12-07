using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Apply
{
    public class FinancialController : Controller
    {
        private readonly ApplyApiClient _apiClient;

        public FinancialController(ApplyApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        
        
        [HttpGet("/Financial/Dashboard")]
        public IActionResult Dashboard()
        {
            return View("~/Views/Apply/Financial/Dashboard.cshtml");
        }

        [HttpGet("/Financial/NewApplications")]
        public async Task<IActionResult> NewApplications()
        {
            var applications = await _apiClient.GetNewFinancialApplications();

            var applicationViewModels = applications.Select(a =>
                new NewFinancialApplicationViewModel
                {
                    ApplicationId = a.applicationId,
                    ApplyingOrganisationName = a.applyingOrganisationName,
                    Status = a.status,
                    LinkText = a.status == "Submitted" ? "Review application" : "Continue"
                }).ToList();
            
            
            return View("~/Views/Apply/Financial/NewApplications.cshtml", applicationViewModels);
        }

        public IActionResult PreviousApplications()
        {
            return View("~/Views/Apply/Financial/PreviousApplications.cshtml");
        }

        [HttpGet("/Financial/{applicationId}")]
        public async Task<IActionResult> ViewApplication(Guid applicationId)
        {
            await _apiClient.StartFinancialReview(applicationId);
            
            var financialSectionId = 3;
            var stage1SequenceId = 1;
            var financialSection = await _apiClient.GetSection(applicationId, stage1SequenceId, financialSectionId);

            var organisation = await _apiClient.GetOrganisationForApplication(applicationId);
            
            var vm = new FinancialApplicationViewModel
            {
                Organisation = organisation,
                Section = financialSection,
                ApplicationId = applicationId
            };
            
            return View("~/Views/Apply/Financial/Application.cshtml", vm);
        }

        [HttpGet("/Financial/Download/{applicationId}/Page/{pageId}/QuestionId/{questionId}")]
        public async Task<IActionResult> Download(Guid applicationId, int pageId, string questionId)
        {
            var section = await _apiClient.GetSection(applicationId, 1, 3);

            var answer = section.QnADataObject.Pages.SelectMany(p => p.PageOfAnswers).SelectMany(a => a.Answers).SingleOrDefault(a => a.QuestionId == questionId);

            var fileDownloadName = answer.Value;
            
            var downloadedFile = await _apiClient.DownloadFile(applicationId, pageId, questionId, Guid.Empty, 1, 3, fileDownloadName);

            var stream = await downloadedFile.Content.ReadAsStreamAsync();
            var contentType = downloadedFile.Content.Headers.ContentType.ToString();
            return File(stream, contentType, fileDownloadName);
        }

        [HttpPost("/Financial/Grade")]
        public async Task<IActionResult> Grade(FinancialApplicationViewModel vm)
        {
            await _apiClient.UpdateFinancialGrade(vm.ApplicationId, vm.Grade);
            return RedirectToAction("Graded", new {vm.ApplicationId});
        }

        [HttpGet("/Financial/Graded")]
        public async Task<IActionResult> Graded(Guid applicationId)
        {
            var section = await _apiClient.GetSection(applicationId, 1, 3);

            return View("~/Views/Apply/Financial/Graded.cshtml", section);
        }
    }

    public class FinancialApplicationViewModel
    {
        public FinancialApplicationViewModel()
        {
            Grade = new FinancialApplicationGrade();
        }
        
        public ApplicationSection Section { get; set; }
        public FinancialApplicationGrade Grade { get; set; }
        public Guid ApplicationId { get; set; }
        public Organisation Organisation { get; set; }
    }

    public class NewFinancialApplicationViewModel
    {
        public string ApplyingOrganisationName { get; set; }
        public string Status { get; set; }
        public string LinkText { get; set; }
        public Guid ApplicationId { get; set; }
    }
}