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
    [Route("certificate/version")]
    public class CertificateVersionNotApprovedController : CertificateBaseController
    {
        public CertificateVersionNotApprovedController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, IStandardVersionClient standardVersionClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
        }

        [HttpGet("not-approved")]
        public IActionResult NotApprovedToAssess(bool? redirectToCheck = false)
        {
            var sessionString = SessionService.Get(nameof(CertificateSession));
            if (sessionString == null)
            {
                return RedirectToAction("Index", "Search");
            }

            var version = SessionService.Get("AttemptedStandardVersion");

            var viewModel = new CertificateVersionNotApprovedViewModel()
            {
                AttemptedVersion = version,
                BackToCheckPage = redirectToCheck.Value
            };
           
            return View("~/Views/Certificate/VersionNotApproved.cshtml", viewModel);
        }
    }
}
