using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/alreadysubmitted")]
    public class CertificateAlreadySubmittedController : Controller
    {
        private readonly ICertificateApiClient _certificateApiClient;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;

        public CertificateAlreadySubmittedController(ICertificateApiClient certificateApiClient,
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            ILogger<CertificateController> logger)
        {
            _certificateApiClient = certificateApiClient;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid id)
        {
            var certificate = await _certificateApiClient.GetCertificate(id);
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
//            var submittedAt = certificate.CertificateLogs.FirstOrDefault(q => q.Status == CertificateStatus.Submitted).EventTime;
            var standard = await _assessmentOrgsApiClient.GetStandard(certificate.StandardCode);
            
            var selectedStandardViewModel = new SelectedStandardViewModel()
            {
                Standard = standard.Title,
                StdCode = certificate.StandardCode.ToString(),
                Uln = certificate.Uln.ToString(),
                GivenNames = certificateData.LearnerGivenNames,
                FamilyName = certificateData.LearnerFamilyName,
                CertificateReference = certificate.CertificateReference,
                OverallGrade = certificateData.OverallGrade,
                Level = certificateData.StandardLevel.ToString(),
//                SubmittedAt = submittedAt.ToString(),
                SubmittedBy = certificate.UpdatedBy,
                LearnerStartDate = certificateData.LearningStartDate.GetValueOrDefault().ToString("d MMMM yyyy"),
                AchievementDate = certificateData.AchievementDate.GetValueOrDefault().ToString("d MMMM yyyy"),
                ShowExtraInfo = true
            };
            return View("~/Views/Certificate/CertificatePrivateResultAlreadyRecorded.cshtml", selectedStandardViewModel);            
        }          
        
        private string GetSubmittedAtString(DateTime? submittedAt)
        {
            return !submittedAt.HasValue ? "" : submittedAt.Value.ToString("d MMMM yyyy");
        }
    }
} 