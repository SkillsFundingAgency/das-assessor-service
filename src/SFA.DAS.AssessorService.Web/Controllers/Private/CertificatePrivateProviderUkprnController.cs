using System.Linq;
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
    [Route("certificate/ukprns")]
    public class CertificatePrivateProviderUkprnController : CertificateBaseController
    {
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;

        public CertificatePrivateProviderUkprnController(ILogger<CertificateController> logger, 
            IHttpContextAccessor contextAccessor,
            IAssessmentOrgsApiClient assessmentOrgsApiClient,
            ICertificateApiClient certificateApiClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Ukprn(bool? redirectToCheck = false)
        {

            var ukprns = (await _assessmentOrgsApiClient.GetProviders())
                .Select(q => new SelectListItem {Value = q.Ukprn.ToString(), Text = q.Ukprn.ToString()});
           

            var viewResult = await LoadViewModel<CertificateUkprnListViewModel>("~/Views/Certificate/Ukprn.cshtml");
            var vm = ((viewResult as ViewResult).Model) as CertificateUkprnListViewModel;
            vm.Ukprns = ukprns;

            return viewResult;
        }

        [HttpPost(Name = "Ukprn")]
        public async Task<IActionResult> Ukprn(CertificateUkprnListViewModel vm)
        {

            var ukprns = (await _assessmentOrgsApiClient.GetProviders())
                .Select(q => new SelectListItem { Value = q.Ukprn.ToString(), Text = q.Ukprn.ToString() });
            vm.Ukprns = ukprns;

            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Ukprn.cshtml",
                nextAction: RedirectToAction("LearnerStartDate", "CertificatePrivateLearnerStartDate"), action: CertificateActions.LearningStartDate);
        }
    }
}