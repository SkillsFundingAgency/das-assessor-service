using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
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
            var certificateDetails = await _certificateApiClient.GetAlreadySubmittedPrivateCertificate(id);
            var selectedStandardViewModel = Mapper.Map<SelectedStandardViewModel>(certificateDetails);
            
            return View("~/Views/Certificate/CertificatePrivateResultAlreadyRecorded.cshtml", selectedStandardViewModel);            
        }                 
    }
} 