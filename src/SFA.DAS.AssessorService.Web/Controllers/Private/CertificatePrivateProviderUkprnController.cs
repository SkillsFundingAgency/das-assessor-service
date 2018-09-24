using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public CertificatePrivateProviderUkprnController(ILogger<CertificateController> logger,
            IHttpContextAccessor contextAccessor,         
            ICertificateApiClient certificateApiClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        {     
        }

        [HttpGet]
        public async Task<IActionResult> Ukprn(bool? redirectToCheck = false)
        {
            var viewResult = await LoadViewModel<CertificateUkprnViewModel>("~/Views/Certificate/Ukprn.cshtml");
            return viewResult;
        }

        [HttpPost(Name = "Ukprn")]
        public async Task<IActionResult> Ukprn(CertificateUkprnViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Ukprn.cshtml",
                nextAction: RedirectToAction("Grade", "CertificateGrade"), action: CertificateActions.Ukprn);
        }
    }
}