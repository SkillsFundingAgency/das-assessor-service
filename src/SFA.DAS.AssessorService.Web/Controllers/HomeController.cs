using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SFA.DAS.AssessorService.Web.Models;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDistributedCache _cache;

        public HomeController(IDistributedCache cache)
        {
            _cache = cache;
        }

        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Shutter()
        {
            return View();
        }

        public IActionResult NotRegistered()
        {
            return View();
        }

        public IActionResult InvalidRole()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }

        public IActionResult Cookies()
        {
            return View();
        }

    }
}
