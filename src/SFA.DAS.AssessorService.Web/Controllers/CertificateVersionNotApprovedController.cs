using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/version")]
    public class CertificateVersionNotApprovedController : CertificateBaseController
    {
        private readonly IStandardVersionClient _standardVersionClient;
        public CertificateVersionNotApprovedController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, IStandardVersionClient standardVersionClient, ISessionService sessionService)
            : base(logger, contextAccessor, certificateApiClient, sessionService)
        {
            _standardVersionClient = standardVersionClient;
        }

        [HttpGet("not-approved")]
        public async Task<IActionResult> NotApprovedToAssess(bool? redirectToCheck = false)
        {
            var sessionString = SessionService.Get(nameof(CertificateSession));
            if (sessionString == null)
            {
                return RedirectToAction("Index", "Search");
            }

            var standardUId = SessionService.Get("AttemptedStandardVersion");

            var standardVersion = await _standardVersionClient.GetStandardVersionByStandardUId(standardUId);

            var viewModel = new CertificateVersionNotApprovedViewModel()
            {
                AttemptedVersion = standardVersion.Version,
                BackToCheckPage = redirectToCheck.Value
            };
           
            return View("~/Views/Certificate/VersionNotApproved.cshtml", viewModel);
        }
    }
}
