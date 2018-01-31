using System.Web.Mvc;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}