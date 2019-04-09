using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Web.Staff.Domain;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Financial;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

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
        
        [HttpGet("/Financial/Open")]
        public async Task<IActionResult> OpenApplications(int page = 1)
        {
            var applications = await _apiClient.GetOpenFinancialApplications();

            var paginatedApplications = new PaginatedList<FinancialApplicationSummaryItem>(applications, applications.Count(), page, int.MaxValue);

            var viewmodel = new FinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Financial/OpenApplications.cshtml", viewmodel);
        }

        [HttpGet("/Financial/Rejected")]
        public async Task<IActionResult> RejectedApplications(int page = 1)
        {
            // NOTE: Rejected actually means Feedback Added or it was graded as Inadequate
            var applications = await _apiClient.GetFeedbackAddedFinancialApplications();

            var paginatedApplications = new PaginatedList<FinancialApplicationSummaryItem>(applications, applications.Count(), page, int.MaxValue);

            var viewmodel = new FinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Financial/RejectedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Financial/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var applications = await _apiClient.GetClosedFinancialApplications();

            var paginatedApplications = new PaginatedList<FinancialApplicationSummaryItem>(applications, applications.Count(), page, int.MaxValue);

            var viewmodel = new FinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Financial/ClosedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Financial/{applicationId}")]
        public async Task<IActionResult> ViewApplication(Guid applicationId)
        {
            await _apiClient.StartFinancialReview(applicationId);
            
            var financialSectionId = 3;
            var stage1SequenceId = 1;
            var financialSection = await _apiClient.GetSection(applicationId, stage1SequenceId, financialSectionId);

            var grade = financialSection?.QnAData?.FinancialApplicationGrade;
            var application = await _apiClient.GetApplication(applicationId);

            var vm = new FinancialApplicationViewModel(applicationId, financialSection, grade, application);
            
            return View("~/Views/Apply/Financial/Application.cshtml", vm);
        }
        
        [HttpGet("/Financial/{applicationId}/Graded")]
        public async Task<IActionResult> ViewGradedApplication(Guid applicationId)
        {
            var financialSectionId = 3;
            var stage1SequenceId = 1;
            var financialSection = await _apiClient.GetSection(applicationId, stage1SequenceId, financialSectionId);

            var grade = financialSection?.QnAData?.FinancialApplicationGrade;
            var application = await _apiClient.GetApplication(applicationId);

            var vm = new FinancialApplicationViewModel(applicationId, financialSection, grade, application);
            
            return View("~/Views/Apply/Financial/Application_ReadOnly.cshtml", vm);
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
                            var answers = section.QnAData.Pages.SelectMany(p => p.PageOfAnswers)
                                .SelectMany(a => a.Answers)
                                .Where(a => a.QuestionId == uploadQuestion.QuestionId && !string.IsNullOrWhiteSpace(a.Value)).ToList();

                            foreach (var answer in answers)
                            {
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
                }
                
                zipStream.Position = 0;

                var compressedBytes = zipStream.ToArray();
                
                return File(compressedBytes, "application/zip", "FinancialDocuments.zip");
            }
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

                var grade = new FinancialApplicationGrade
                {
                    SelectedGrade = vm.Grade.SelectedGrade,
                    OutstandingFinancialDueDate = vm.Grade.OutstandingFinancialDueDate,
                    GoodFinancialDueDate = vm.Grade.GoodFinancialDueDate,
                    SatisfactoryFinancialDueDate = vm.Grade.SatisfactoryFinancialDueDate
                };
                var application = await _apiClient.GetApplication(vm.ApplicationId);

                var newvm = new FinancialApplicationViewModel(vm.ApplicationId, financialSection, grade, application);
                return View("~/Views/Apply/Financial/Application.cshtml", newvm);
            }
        }

        private static string GetEpaOrgId(Organisation org)
        {
            var referenceId = org.OrganisationDetails.OrganisationReferenceId;
            if (string.IsNullOrEmpty(referenceId) || !referenceId.Contains(","))
            {
                return referenceId;                
            }

            var ids = referenceId.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return ids.First(i => i.StartsWith("EPA"));
        }

        private static void GetFinancialDueDate(FinancialApplicationViewModel vm)
        {
            switch (vm?.Grade?.SelectedGrade)
            {
                case FinancialApplicationSelectedGrade.Outstanding:
                    vm.Grade.FinancialDueDate = vm.Grade.OutstandingFinancialDueDate.ToDateTime();
                    break;
                case FinancialApplicationSelectedGrade.Good:
                    vm.Grade.FinancialDueDate = vm.Grade.GoodFinancialDueDate.ToDateTime();
                    break;
                case FinancialApplicationSelectedGrade.Satisfactory:
                    vm.Grade.FinancialDueDate = vm.Grade.SatisfactoryFinancialDueDate.ToDateTime();
                    break;
                case null:
                default:
                    break;

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