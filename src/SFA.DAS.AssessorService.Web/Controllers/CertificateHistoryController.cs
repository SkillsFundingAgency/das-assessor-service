using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
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
            var username = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var certificateHistory = await _certificateApiClient.GetCertificateHistory(pageIndex ?? 1, username);            
            return View("Index", certificateHistory);
        }
    }
}