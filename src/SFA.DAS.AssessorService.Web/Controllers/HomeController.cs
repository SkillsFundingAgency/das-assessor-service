﻿using System.Diagnostics;
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
        public IActionResult Index()
        {
            _cache.SetString("Hello", "Dave");
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult NotRegistered()
        {
            return View();
        }

        public IActionResult InvalidRole()
        {
            return View();
        }
    }
}
