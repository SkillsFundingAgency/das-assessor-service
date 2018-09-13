using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    public class ApplyController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Apply/Apply.cshtml");
        }

        public IActionResult StartApplicant()
        {
            HttpContext.Session.SetString("Actor", "Applicant");
            return RedirectToAction("Index", "Sequence");
        }
        
        public IActionResult StartAdmin()
        {
            HttpContext.Session.SetString("Actor", "Admin");
            return RedirectToAction("Index", "Sequence");
        }
    }
}