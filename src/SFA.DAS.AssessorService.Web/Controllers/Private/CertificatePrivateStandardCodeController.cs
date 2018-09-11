﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/privatestandardcodes")]
    public class CertificatePrivateStandardCodeController : CertificateBaseController
    {
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly ICertificateApiClient _certificateApiClient;
        private readonly ISessionService _sessionService;

        public CertificatePrivateStandardCodeController(ILogger<CertificateController> logger,
            IHttpContextAccessor contextAccessor,
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            ICertificateApiClient certificateApiClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _certificateApiClient = certificateApiClient;
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> StandardCode(bool? redirectToCheck = false)
        {
            var standardCodes = (await _assessmentOrgsApiClient.GetAllStandards())
                .Select(q => new SelectListItem { Value = q.Id.ToString(), Text = q.Title.ToString() });

            var viewResult = await LoadViewModel<CertificateStandardCodeListViewModel>("~/Views/Certificate/StandardCode.cshtml");
            var vm = ((viewResult as ViewResult).Model) as CertificateStandardCodeListViewModel;
            vm.StandardCodes = standardCodes;

            return viewResult;
        }

        [HttpPost(Name = "StandardCode")]
        public async Task<IActionResult> StandardCode(CertificateStandardCodeListViewModel vm)
        {
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var standards = (await _assessmentOrgsApiClient.GetAllStandards());

            var selectedStandard = standards.First(q => q.Id == vm.SelectedStandardCode);
            vm.Standard = selectedStandard.Title;
            vm.Level = selectedStandard.Level;

            var standardCodes = standards
                .Select(q => new SelectListItem { Value = q.Id.ToString(), Text = q.Title.ToString() });
            vm.StandardCodes = standardCodes;

            var sessionString = SessionService.Get("CertificateSession");
            if (sessionString == null)
            {
                Logger.LogInformation($"Session for CertificateOptionViewModel requested by {username} has been lost. Redirecting to Search Index");
                return RedirectToAction("Index", "Search");
            }
            var certificateSession = JsonConvert.DeserializeObject<CertificateSession>(sessionString);

            var options = (await _certificateApiClient.GetOptions(vm.SelectedStandardCode)).Select(o => o.OptionName).ToList();
            certificateSession.Options = options;

            _sessionService.Set("CertificateSession", new CertificateSession()
            {
                CertificateId = certificateSession.CertificateId,
                Uln = certificateSession.Uln,
                StandardCode = vm.SelectedStandardCode,
                Options = options
            });

            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/StandardCode.cshtml",
                nextAction: RedirectToAction("LearnerStartDate", "CertificatePrivateLearnerStartDate"), action: CertificateActions.StatusCode);
        }
    }
}