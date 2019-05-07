namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    public class ErrorPageController : Controller
    {
        [Route("ErrorPage/404")]
        public async Task<IActionResult> PageNotFound()
        {
            return View("~/Views/ErrorPage/PageNotFound.cshtml");
        }
        
        [Route("ErrorPage/500")]
        public async Task<IActionResult> ServiceErrorHandler()
        {
            return RedirectToAction("ServiceError");
        }

        [Route("problem-with-service")]
        public async Task<IActionResult> ServiceError()
        {
            return View("~/Views/ErrorPage/ServiceError.cshtml");
        }

        [Route("ErrorPage/503")]
        public async Task<IActionResult> ServiceUnavailableHandler()
        {
            return RedirectToAction("ServiceUnavailable");
        }

        [Route("service-unavailable")]
        public async Task<IActionResult> ServiceUnavailable()
        {
            return View("~/Views/ErrorPage/ServiceUnavailable.cshtml");
        }
    }
}
