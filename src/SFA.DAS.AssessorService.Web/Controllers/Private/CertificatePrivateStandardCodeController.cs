﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
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

        public CertificatePrivateStandardCodeController(ILogger<CertificateController> logger,
            IHttpContextAccessor contextAccessor,
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            ICertificateApiClient certificateApiClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
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
            var standards = (await _assessmentOrgsApiClient.GetAllStandards());

            var selectedStandard = standards.First(q => q.Id == vm.SelectedStandardCode);
            vm.Standard = selectedStandard.Title;
            vm.Level = selectedStandard.Level;

            var standardCodes = standards
                .Select(q => new SelectListItem { Value = q.Id.ToString(), Text = q.Title.ToString() });
            vm.StandardCodes = standardCodes;

            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/StandardCode.cshtml",
                nextAction: RedirectToAction("Grade", "CertificateGrade"), action: CertificateActions.StatusCode);
        }
    }
}