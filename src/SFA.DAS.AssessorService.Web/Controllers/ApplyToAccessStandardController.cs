using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class ApplyToAccessStandardController : Controller
    {

        private readonly IWebConfiguration _webConfiguration;

        public ApplyToAccessStandardController(IWebConfiguration webConfiguration)
        {
            _webConfiguration = webConfiguration;
        }

        [HttpGet]
        [Route("ApplyToAccessStandard")]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Standards })]
        public IActionResult Index()
        {
            TempData["applyToAssessAStandard"] = $"{_webConfiguration.ApplyBaseAddress}/Applications";
            return View();
        }
    }
}