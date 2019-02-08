using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.Web.Staff.Domain;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Financial;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Apply
{
    [Authorize(Roles = Roles.ProviderRiskAssuranceTeam + "," + Roles.CertificationTeam)]
    public class FinancialController : Controller
    {
        private readonly ApplyApiClient _apiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApiClient _assessorApiClient;

        public FinancialController(ApplyApiClient apiClient, IHttpContextAccessor contextAccessor, ApiClient assessorApiClient)
        {
            _apiClient = apiClient;
            _contextAccessor = contextAccessor;
            _assessorApiClient = assessorApiClient;
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
                    Status = a.status
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
                ApplicationId = applicationId,
                Grade = new FinancialApplicationGrade()
                {
                    OutstandingFinancialDueDate = new FinancialDueDate(),
                    GoodFinancialDueDate = new FinancialDueDate(),
                    SatisfactoryFinancialDueDate = new FinancialDueDate()
                }
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
                Grade = financialSection.QnAData.FinancialApplicationGrade
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
                            var answer = section.QnAData.Pages.SelectMany(p => p.PageOfAnswers).SelectMany(a => a.Answers).SingleOrDefault(a => a.QuestionId == uploadQuestion.QuestionId);

                            if (answer == null || string.IsNullOrWhiteSpace(answer.Value)) continue;
                            
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

                GetFinancialDueDate(vm);
                
                await _apiClient.UpdateFinancialGrade(vm.ApplicationId, vm.Grade);

                var org = await _apiClient.GetOrganisationForApplication(vm.ApplicationId);
                
                if (org.RoEPAOApproved)
                {
                    await _assessorApiClient.UpdateFinancials(new UpdateFinancialsRequest
                    {
                        EpaOrgId = GetEpaOrgId(org),
                        FinancialDueDate = vm.Grade.FinancialDueDate,
                        FinancialExempt = vm.Grade.SelectedGrade == "Exempt"
                    });
                }

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
                newvm.Grade.OutstandingFinancialDueDate = vm.Grade.OutstandingFinancialDueDate;
                newvm.Grade.GoodFinancialDueDate = vm.Grade.GoodFinancialDueDate;
                newvm.Grade.SatisfactoryFinancialDueDate = vm.Grade.SatisfactoryFinancialDueDate;
                return View("~/Views/Apply/Financial/Application.cshtml", newvm);
            }
        }

        private static string GetEpaOrgId(Organisation org)
        {
            var referenceId = org.OrganisationDetails.OrganisationReferenceId;
            if (!referenceId.Contains(","))
            {
                return referenceId;                
            }

            var ids = referenceId.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return ids.First(i => i.StartsWith("EPA"));
        }

        private static void GetFinancialDueDate(FinancialApplicationViewModel vm)
        {
            if (vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Excellent)
            {
                vm.Grade.FinancialDueDate = vm.Grade.OutstandingFinancialDueDate.ToDateTime();
            }
            else if (vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Good)
            {
                vm.Grade.FinancialDueDate = vm.Grade.GoodFinancialDueDate.ToDateTime();
            }
            else if (vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Satisfactory)
            {
                vm.Grade.FinancialDueDate = vm.Grade.SatisfactoryFinancialDueDate.ToDateTime();
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