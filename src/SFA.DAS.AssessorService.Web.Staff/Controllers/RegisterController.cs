﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize]
    public class RegisterController: Controller
    {
        private readonly ApiClient _apiClient;

        public RegisterController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public IActionResult Index()
        {
            return View(); 
        }

        [HttpGet("register/results")]
        public async Task<IActionResult> Results(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("index",vm);
            }

            var searchstring = vm.SearchString?.Trim().ToLower();
            searchstring = string.IsNullOrEmpty(searchstring) ? "" : searchstring;
            var rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
            searchstring = rx.Replace(searchstring, "");
            var searchResults = await _apiClient.SearchOrganisations(searchstring);

            var registerViewModel = new RegisterViewModel
            {
                Results = searchResults,
                SearchString = vm.SearchString
            };

            return View(registerViewModel);
        }

        [HttpGet("register/impage")]
        public async Task<IActionResult> Impage()
        {
            var vm = new AssessmentOrgsImportResponse { Status = "Press to run" };
            return View(vm);
        }

        [HttpGet("register/impage-{choice}")]
        public async Task<IActionResult> Impage(string choice)
        {
            var vm = new AssessmentOrgsImportResponse { Status = "Running" };

            if (choice == "DoIt")
            {
                var importResults = await _apiClient.ImportOrganisations();
                vm.Status = importResults;
            }
            return View(vm);
        }
    }
}
