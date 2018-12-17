using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.AssessorService.Web.Staff.Domain;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Apply
{
    [Authorize(Roles = Roles.ProviderRiskAssuranceTeam + "," + Roles.CertificationTeam)]
    public class FinancialController : Controller
    {
        private readonly ApplyApiClient _apiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public FinancialController(ApplyApiClient apiClient, IHttpContextAccessor contextAccessor)
        {
            _apiClient = apiClient;
            _contextAccessor = contextAccessor;
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

        [HttpGet("/Financial/PreviousApplications")]
        public async Task<IActionResult> PreviousApplications()
        {
            var applications = await _apiClient.GetPreviousFinancialApplications();

            var applicationViewModels = applications.Select(a =>
                new PreviousFinancialApplicationViewModel
                {
                    ApplicationId = a.applicationId,
                    ApplyingOrganisationName = a.applyingOrganisationName,
                    LinkText = "View application",
                    Grade = a.grade,
                    GradedBy = a.gradedBy,
                    GradedDate = DateTime.Parse(a.gradedDateTime.ToString())
                }).ToList();
            
            return View("~/Views/Apply/Financial/PreviousApplications.cshtml", applicationViewModels);
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
        
        [HttpGet("/Financial/{applicationId}/Graded")]
        public async Task<IActionResult> ViewGradedApplication(Guid applicationId)
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
                ApplicationId = applicationId,
                Grade = financialSection.QnADataObject.FinancialApplicationGrade
            };
            
            return View("~/Views/Apply/Financial/PreviousApplication.cshtml", vm);
        }

        [HttpGet("/Financial/Download/{applicationId}")]
        public async Task<IActionResult> Download(Guid applicationId)
        {
            var section = await _apiClient.GetSection(applicationId, 1, 3);

            using (var zipStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    foreach (var uploadPage in section.PagesContainingUploadQuestions)
                    {
                        foreach (var uploadQuestion in uploadPage.UploadQuestions)
                        {
                            var answer = section.QnADataObject.Pages.SelectMany(p => p.PageOfAnswers).SelectMany(a => a.Answers).SingleOrDefault(a => a.QuestionId == uploadQuestion.QuestionId);

                            var fileDownloadName = answer.Value;
            
                            var downloadedFile = await _apiClient.DownloadFile(applicationId, int.Parse(uploadPage.PageId), uploadQuestion.QuestionId, Guid.Empty, 1, 3, fileDownloadName);

                            var zipEntry = zipArchive.CreateEntry(fileDownloadName);
                            using (var entryStream = zipEntry.Open())
                            {
                                var fileStream = await downloadedFile.Content.ReadAsStreamAsync();
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }
                }
                
                zipStream.Position = 0;

                var compressedBytes = zipStream.ToArray();
                
                return File(compressedBytes, "application/zip", "FinancialDocuments.zip");
            }
            
            
            

            
//            var stream = await downloadedFile.Content.ReadAsStreamAsync();
//            var contentType = downloadedFile.Content.Headers.ContentType.ToString();
        }

        [HttpPost("/Financial/{applicationId}")]
        public async Task<IActionResult> Grade(Guid applicationId, FinancialApplicationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var givenName = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")?.Value;
                var surname = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")?.Value;

                vm.Grade.GradedBy = $"{givenName} {surname}";
            
                await _apiClient.UpdateFinancialGrade(vm.ApplicationId, vm.Grade);
                return RedirectToAction("Graded", new {vm.ApplicationId});   
            }
            else
            {
                var financialSectionId = 3;
                var stage1SequenceId = 1;
                var financialSection = await _apiClient.GetSection(vm.ApplicationId, stage1SequenceId, financialSectionId);

                var organisation = await _apiClient.GetOrganisationForApplication(vm.ApplicationId);
            
                var newvm = new FinancialApplicationViewModel
                {
                    Organisation = organisation,
                    Section = financialSection,
                    ApplicationId = vm.ApplicationId
                };
                newvm.Grade.SelectedGrade = vm.Grade.SelectedGrade;
                return View("~/Views/Apply/Financial/Application.cshtml", newvm);
            }
        }

        [HttpGet("/Financial/Graded")]
        public async Task<IActionResult> Graded(Guid applicationId)
        {
            var section = await _apiClient.GetSection(applicationId, 1, 3);

            return View("~/Views/Apply/Financial/Graded.cshtml", section);
        }
    }
}