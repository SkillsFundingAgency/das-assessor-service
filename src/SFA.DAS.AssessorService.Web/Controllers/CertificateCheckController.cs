using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/check")]
    public class CertificateCheckController : CertificateBaseController
    {
        public CertificateCheckController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient) : base(logger, contextAccessor, certificateApiClient)
        {}

        [HttpGet]
        public async Task<IActionResult> Check()
        {
            return await LoadViewModel<CertificateCheckViewModel>("~/Views/Certificate/Check.cshtml");
        }
        
        [HttpPost(Name = "Check")]
        public async Task<IActionResult> Check(CertificateCheckViewModel vm)
        {
            return await SaveViewModel(vm, 
                returnToIfModelNotValid: "~/Views/Certificate/Check.cshtml",
                nextAction: RedirectToAction("Confirm", "CertificateConfirmation"));
        }
    }
}