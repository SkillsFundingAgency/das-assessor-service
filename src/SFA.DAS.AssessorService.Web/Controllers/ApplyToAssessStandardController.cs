using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [PrivilegeAuthorize(Privileges.ApplyForStandard)]
    [CheckSession]
    public class ApplyToAssessStandardController : Controller
    {
        #region Routes
        public const string ApplyToAssessStandardRouteGet = nameof(ApplyToAssessStandardRouteGet);
        #endregion

        public ApplyToAssessStandardController()
        {
        }

        [HttpGet("ApplyToAssessStandard", Name = ApplyToAssessStandardRouteGet)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Standards })]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("GoToApplyToAssessStandard")]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Standards })]
        public IActionResult GoToApplyToAssessStandard()
        {
            return Redirect($"/Application");
        }
    }
}