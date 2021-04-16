using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [Route("certificate/declaration")]
    public class CertificateDeclarationController : CertificateBaseController
    {
        public CertificateDeclarationController(ILogger<CertificateController> logger, IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient, ISessionService sessionService) : base(logger, contextAccessor, certificateApiClient, sessionService)
        { }

        [HttpGet]
        public async Task<IActionResult> Declare()
        {
            return await LoadViewModel<CertificateDeclarationViewModel>("~/Views/Certificate/Declaration.cshtml");
        }

        [HttpPost(Name = "Declare")]
        public async Task<IActionResult> Declare(CertificateDeclarationViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/Certificate/Declaration.cshtml",
                nextAction: RedirectToAction("Grade", "CertificateGrade"), action: CertificateActions.Declaration);
        }

        [HttpGet("back", Name = "Back")]
        public IActionResult Back()
        {
            var username = GetUsernameFromClaim();
            if (!TryGetCertificateSession("CertificateDeclarationViewModel", username, out CertificateSession certSession))
            {
                return RedirectToAction("Index", "Search");
            }
            
            var hasOptions = certSession.Options != null && certSession.Options.Any();
            var hasVersions = certSession.Versions != null && certSession.Versions.Any();

            if (hasOptions)
            {
                if (certSession.Options.Count == 1)
                {
                    return RedirectToAction("Result", "Search");
                }

                return RedirectToAction("Option", "CertificateOption");
            }

            if (hasVersions)
            {
                if (certSession.Versions.Count == 1)
                {
                    return RedirectToAction("Result", "Search");
                }

                return RedirectToAction("Version", "CertificateVersion");
            }

            // No Options and No Version, return to search.
            return RedirectToAction("Index", "Search");
        }
    }
}