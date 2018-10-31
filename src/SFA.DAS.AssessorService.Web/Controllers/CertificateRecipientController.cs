using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/recipient")]
    public class CertificateRecipientController : CertificateBaseController
    {
        public CertificateRecipientController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {}

        [HttpGet]
        public async Task<IActionResult> Recipient(bool? redirectToCheck = false,
            bool? redirecttosearch = false)
        {
            return await LoadViewModel<CertificateRecipientViewModel>("~/Views/Certificate/Recipient.cshtml");
        }
        
        [HttpPost(Name = "Recipient")]
        public async Task<IActionResult> Recipient(CertificateRecipientViewModel vm)
        {
            return await SaveViewModel(vm, 
                returnToIfModelNotValid: "~/Views/Certificate/Recipient.cshtml",
                nextAction: RedirectToAction("Check","CertificateCheck"), action: CertificateActions.Recipient);
        }
    }
}