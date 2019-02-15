namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class RoatpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}
