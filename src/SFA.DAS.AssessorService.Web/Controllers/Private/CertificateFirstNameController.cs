using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/firstname")]
    public class CertificateFirstNameController : CertificateBaseController
    {
        public CertificateFirstNameController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor, ICertificateApiClient certificateApiClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        { }

        [HttpGet]
        public async Task<IActionResult> FirstName(bool? redirectToCheck = false)
        {
            CertificateFirstNameViewModel vm = new CertificateFirstNameViewModel();
            return await LoadViewModel<CertificateFirstNameViewModel>("~/Views/Certificate/FirstName.cshtml");
        }

        [HttpPost(Name = "FirstName")]
        public async Task<IActionResult> FirstName(CertificateFirstNameViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/FirstName.cshtml",
                nextAction: RedirectToAction("Ukprn", "CertificateProviderUkprn"), action: CertificateActions.FirstName);
        }
    }
}