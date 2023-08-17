using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Models;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IWebConfiguration _webConfiguration;

        public HomeController(ISessionService sessionService, IWebConfiguration webConfiguration)
        {
            _sessionService = sessionService;
            _webConfiguration = webConfiguration;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            // store the UseGovSignIn property value in the Session so that it could be used in the layout.
            _sessionService.Set(nameof(WebConfiguration.UseGovSignIn), _webConfiguration.UseGovSignIn);

            return View(new HomeIndexViewModel { UseGovSignIn = _webConfiguration.UseGovSignIn });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult NotRegistered()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> NotActivated()
        {
            return View();
        }

        [Authorize]
        public IActionResult InvalidRole()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult Cookies()
        {
            return View();
        }

        public IActionResult CookieDetails()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Accessibility()
        {
            return View();
        }

        [Authorize]
        [CheckSession]
        public IActionResult InvitePending()
        {
            return View(model: _sessionService.Get("OrganisationName"));
        }

        [Authorize]
        [CheckSession]
        public IActionResult Rejected()
        {
            return View(model: _sessionService.Get("OrganisationName"));
        }
    }
}
