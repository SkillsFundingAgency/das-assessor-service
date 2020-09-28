using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [PrivilegeAuthorize(Privileges.ViewCompletedAssessments)]
    [CheckSession]
    [Route("[controller]/[action]")]
    public class CertificateHistoryController : Controller
    {
        private readonly ILogger<CertificateController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICertificateApiClient _certificateApiClient;        
        private readonly ISessionService _sessionService;

        public CertificateHistoryController(
            ILogger<CertificateController> logger,
            IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient,        
            ISessionService sessionService)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _certificateApiClient = certificateApiClient;
            _sessionService = sessionService;       
        }

        [HttpGet]
        [Route("/[controller]/")]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Assessments })]
        public async Task<IActionResult> Index(int? pageIndex)
        {
            var endPointAssessorOrganisationId = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;

            var model = new CertificateHistoryViewModel
            { 
                Certificates = await _certificateApiClient.GetCertificateHistory(pageIndex ?? 1, endPointAssessorOrganisationId)
            };

            return View("Index", model);
        }
    }
}