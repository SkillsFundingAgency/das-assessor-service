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
    [Route("certificate/address")]
    public class CertificateAddressController : CertificateBaseController
    {
        public CertificateAddressController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {}

        [HttpGet]
        public async Task<IActionResult> Address(bool? redirectToCheck = false)
        {
            return await LoadViewModel<CertificateAddressViewModel>("~/Views/Certificate/Address.cshtml");
        }
        
        [HttpPost(Name = "Address")]
        public async Task<IActionResult> Address(CertificateAddressViewModel vm)
        {
            return await SaveViewModel(vm, 
                returnToIfModelNotValid: "~/Views/Certificate/Address.cshtml",
                nextAction: RedirectToAction("Check","CertificateCheck"), action: CertificateActions.Address);
        }
    }
}