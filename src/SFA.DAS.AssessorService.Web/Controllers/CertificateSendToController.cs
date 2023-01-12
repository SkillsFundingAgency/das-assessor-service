using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/sendto")]
    public class CertificateSendToController : CertificateBaseController
    {
        public CertificateSendToController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> SendTo()
        {
            return await LoadViewModel<CertificateSendToViewModel>("~/Views/Certificate/SendTo.cshtml");
        }

        [HttpPost(Name = "SendTo")]
        public async Task<IActionResult> SendTo(CertificateSendToViewModel vm)
        {
            var nextAction = vm.SendTo == CertificateSendTo.Apprentice
                ? RedirectToAction("Address", "CertificateAddress")
                : RedirectToAction("PreviousAddress", "CertificateAddress");

            var certData = await GetCertificateData(vm.Id);
            if (ModelState.IsValid && vm.SendToHasChanged(certData))
            {
                // when recipient has changed the complete journey is required
                SessionService.SetRedirectToCheck(false);
            }

            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/SendTo.cshtml",
                nextAction: nextAction, action: CertificateActions.SendTo);

            return actionResult;
        }
    }
}