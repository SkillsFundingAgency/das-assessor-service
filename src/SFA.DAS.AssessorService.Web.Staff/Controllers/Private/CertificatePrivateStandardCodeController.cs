﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Private
{
    [Authorize]
    [Route("certificate/privatestandardcodes")]
    public class CertificatePrivateStandardCodeController : CertificateBaseController
    {
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly CacheHelper _cacheHelper;
        private readonly ApiClient _apiClient;       

        public CertificatePrivateStandardCodeController(ILogger<CertificateAmmendController> logger,
            IHttpContextAccessor contextAccessor,
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            CacheHelper cacheHelper,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _cacheHelper = cacheHelper;
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> StandardCode(Guid certificateid)
        {
            var filteredStandardCodes = await GetFilteredStatusCodes(certificateid);
            var standards = (await GetAllStandards()).ToList();

            var results = GetSelectListItems(standards, filteredStandardCodes);

            var viewResult = await LoadViewModel<CertificateStandardCodeListViewModel>(certificateid, "~/Views/CertificateAmmend/StandardCode.cshtml");
            if (viewResult is ViewResult)
            {
                var vm = ((viewResult as ViewResult).Model) as CertificateStandardCodeListViewModel;
                vm.StandardCodes = results;
            }

            return viewResult;
        }

        [HttpPost(Name = "StandardCode")]
        public async Task<IActionResult> StandardCode(CertificateStandardCodeListViewModel vm)
        {
            var username = ContextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var filteredStandardCodes = await GetFilteredStatusCodes(vm.Id);
            var standards = (await GetAllStandards()).ToList();

            vm.StandardCodes = GetSelectListItems(standards, filteredStandardCodes);
            if (!string.IsNullOrEmpty(vm.SelectedStandardCode))
            {
                var selectedStandard = standards.First(q => q.Id == Convert.ToInt32(vm.SelectedStandardCode));
                vm.Standard = selectedStandard.Title;
                vm.Level = selectedStandard.Level;
            }

            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmmend/StandardCode.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmmend", new { certificateid = vm.Id }), action: CertificateActions.StatusCode);

            return actionResult;
        }

        private IEnumerable<SelectListItem> GetSelectListItems(List<Standard> standards,
            List<string> filteredStandardCodes)
        {
            return standards
                .Where(a => filteredStandardCodes.Contains(a.Id.ToString()))
                .Select(q => new SelectListItem { Value = q.Id.ToString(), Text = q.Title.ToString() + " (" + q.Id + ')' })
                .ToList()
                .OrderBy(q => q.Text);
        }

        private async Task<List<string>> GetFilteredStatusCodes(Guid certificateid)
        {
            var certificate = await ApiClient.GetCertificate(certificateid);
            var organisation = await ApiClient.GetOrganisation(certificate.OrganisationId);         

            var filteredStandardCodes =
                (await _assessmentOrgsApiClient
                    .FindAllStandardsByOrganisationIdAsync(organisation.EndPointAssessorOrganisationId))
                .Select(q => q.StandardCode).ToList();
            return filteredStandardCodes;
        }

        private async Task<IEnumerable<Standard>> GetAllStandards()
        {
            var results = await _cacheHelper.RetrieveFromCache<IEnumerable<Standard>>("Standards");
            if (results == null)
            {
                var standards = await _assessmentOrgsApiClient.GetAllStandards();
                await _cacheHelper.SaveToCache("Standards", standards, 1);

                results = standards;
            }

            return results;
        }
    }
}