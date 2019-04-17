namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class TestErrorController : Controller
    {
        [Route("test-503-page")]
        public async Task<IActionResult> ServiceUnavailable()
        {
            if (!IsTestHost())
            {
                return await RedirectToHomepage();
            }

            return await Task.FromResult(new StatusCodeResult(503));
        }
        
        [Route("test-500-page")]
        public async Task<IActionResult> ServiceError()
        {
            if (!IsTestHost())
            {
                return await RedirectToHomepage();
            }

            return await Task.FromResult(new StatusCodeResult(500));
        }

        private bool IsTestHost()
        {
            var hostName = ControllerContext.HttpContext.Request.Host.Host;

            return (hostName == "localhost"
                    || hostName.StartsWith("at-manage-assessors")
                    || hostName.StartsWith("test-manage-assessors")
                    || hostName.StartsWith("test2-manage-assessors")
                    || hostName.StartsWith("pp-manage-assessors"));
        }

        private static Task<RedirectToActionResult> RedirectToHomepage()
        {
            return Task.FromResult(new RedirectToActionResult("Index", "Home", null));
        }
    }
}
