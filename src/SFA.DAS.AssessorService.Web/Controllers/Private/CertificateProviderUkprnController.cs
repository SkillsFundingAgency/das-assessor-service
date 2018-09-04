using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/firstname")]
    public class CertificateProviderUkprnController : CertificateBaseController
    {
        public CertificateProviderUkprnController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        { }

        [HttpGet]
        public async Task<IActionResult> Ukprn(bool? redirectToCheck = false)
        {
            CertificateFirstNameViewModel vm = new CertificateFirstNameViewModel();
            return await LoadViewModel<CertificateFirstNameViewModel>("~/Views/Certificate/FirstName.cshtml");
        }

        [HttpPost(Name = "Ukprn")]
        public async Task<IActionResult> Ukprn(CertificateFirstNameViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/FirstName.cshtml",
                nextAction: RedirectToAction("LearnerStartDate", "CertificateLearnerStartDate"), action: CertificateActions.Grade);
        }
    }
}