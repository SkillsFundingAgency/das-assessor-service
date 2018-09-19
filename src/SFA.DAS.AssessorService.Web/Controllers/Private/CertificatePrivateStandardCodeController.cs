using System.Collections.Generic;
using System.Linq;
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
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/privatestandardcodes")]
    public class CertificatePrivateStandardCodeController : CertificateBaseController
    {
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly CacheHelper _cacheHelper;
        private readonly ICertificateApiClient _certificateApiClient;
        private readonly ISessionService _sessionService;

        public CertificatePrivateStandardCodeController(ILogger<CertificateController> logger,
            IHttpContextAccessor contextAccessor,
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            CacheHelper cacheHelper,
            ICertificateApiClient certificateApiClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _cacheHelper = cacheHelper;
            _certificateApiClient = certificateApiClient;
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> StandardCode(bool? redirectToCheck = false)
        {
            var filteredStandardCodes = await GetFilteredStatusCodes();
            var standards = (await GetAllStandards()).ToList();

            var results = GetSelectListItems(standards, filteredStandardCodes);

            var viewResult = await LoadViewModel<CertificateStandardCodeListViewModel>("~/Views/Certificate/StandardCode.cshtml");
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
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var filteredStandardCodes = await GetFilteredStatusCodes();
            var standards = (await GetAllStandards()).ToList();

            vm.StandardCodes = GetSelectListItems(standards, filteredStandardCodes);

            var selectedStandard = standards.First(q => q.Id == vm.SelectedStandardCode);
            vm.Standard = selectedStandard.Title;
            vm.Level = selectedStandard.Level;

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

        private IEnumerable<SelectListItem> GetSelectListItems(List<Standard> standards,
            List<string> filteredStandardCodes)
        {
            return standards
                .Where(a => filteredStandardCodes.Contains(a.Id.ToString()))
                .Select(q => new SelectListItem { Value = q.Id.ToString(), Text = q.Title.ToString() + '(' + q.Id + ')' })
                .ToList()
                .OrderBy(q => q.Text);
        }

        private async Task<List<string>> GetFilteredStatusCodes()
        {
            var endPointAsessorOrganisationId = _sessionService.Get("EndPointAsessorOrganisationId");

            var filteredStandardCodes =
                (await _assessmentOrgsApiClient
                    .FindAllStandardsByOrganisationIdAsync(endPointAsessorOrganisationId))
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