using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AssessorService.Web.Controllers.OppFinder
{
    public class OppFinderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}