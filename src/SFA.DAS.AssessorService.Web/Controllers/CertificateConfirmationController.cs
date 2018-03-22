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
    [Route("certificate/confirmation")]
    public class CertificateConfirmationController : CertificateBaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CertificateConfirmationController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient) : base(logger, contextAccessor, certificateApiClient)
        {
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Confirm()
        {
            var actionResult = await LoadViewModel<CertificateConfirmationViewModel>("~/Views/Certificate/Confirmation.cshtml");

            _contextAccessor.HttpContext.Session.Remove("CertificateSession");

            return actionResult;
        }
    }
}