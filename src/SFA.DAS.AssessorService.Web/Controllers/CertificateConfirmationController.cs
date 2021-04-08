using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/confirmation")]
    public class CertificateConfirmationController : CertificateBaseController
    {
        private readonly ISessionService _sessionService;

        public CertificateConfirmationController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> Confirm()
        {
            var actionResult = await LoadViewModel<CertificateConfirmationViewModel>("~/Views/Certificate/Confirmation.cshtml");

            _sessionService.Remove("CertificateSession");

            return actionResult;
        }
    }
}