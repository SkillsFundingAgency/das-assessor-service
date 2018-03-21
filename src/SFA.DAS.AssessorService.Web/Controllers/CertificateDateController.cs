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
    [Route("certificate/date")]
    public class CertificateDateController : CertificateBaseController
    {
        public CertificateDateController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient) : base(logger, contextAccessor, certificateApiClient)
        {}

        [HttpGet]
        public async Task<IActionResult> Date(bool? redirectToCheck = false)
        {
            return await LoadViewModel<CertificateDateViewModel>("~/Views/Certificate/Date.cshtml");
        }
        
        [HttpPost(Name = "Date")]
        public async Task<IActionResult> Date(CertificateDateViewModel vm)
        {
            return await SaveViewModel(vm, 
                returnToIfModelNotValid: "~/Views/Certificate/Date.cshtml",
                nextAction: RedirectToAction("Address", "CertificateAddress"));
        }
    }
}