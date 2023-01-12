using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/version")]
    public class CertificateVersionNotApprovedController : Controller
    {
        private readonly IStandardVersionClient _standardVersionClient;
        private readonly ISessionService _sessionService;

        public CertificateVersionNotApprovedController(IStandardVersionClient standardVersionClient, ISessionService sessionService)
        {
            _standardVersionClient = standardVersionClient;
            _sessionService = sessionService;
        }

        [HttpGet("not-approved")]
        public async Task<IActionResult> NotApprovedToAssess(bool? redirectToCheck = false)
        {
            var standardUId = _sessionService.Get("AttemptedStandardVersion");

            if (standardUId == null)
            {
                return RedirectToAction("Index", "Search");
            }

            var standardVersion = await _standardVersionClient.GetStandardVersionById(standardUId);

            var viewModel = new CertificateVersionNotApprovedViewModel()
            {
                AttemptedVersion = standardVersion.Version,
                BackToCheckPage = redirectToCheck.Value
            };

            return View("~/Views/Certificate/VersionNotApproved.cshtml", viewModel);
        }
    }
}
