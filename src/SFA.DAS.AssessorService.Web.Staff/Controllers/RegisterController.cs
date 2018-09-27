using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

            var searchResults = await _apiClient.SearchOrganisations(vm.SearchString?.Trim().ToLower());

            var registerViewModel = new RegisterViewModel
            {
                Results = searchResults,
                SearchString = vm.SearchString
            };

            return View(registerViewModel);
        }

        [HttpGet("register/add-organisation")]
        public async Task<IActionResult> AddOrganisation()
        {
            var vm = new RegisterAddOrganisationViewModel
            {
                OrganisationTypes = await _apiClient.GetOrganisationTypes()
            };

            return View(vm);
        }

        [HttpPost("register/add-organisation")]
        public async Task<IActionResult> AddOrganisation(RegisterAddOrganisationViewModel viewModel)
        {
            var vm = new RegisterAddOrganisationViewModel
            {
                OrganisationTypes = await _apiClient.GetOrganisationTypes()
            };

            return View(vm);
        }
    }
}
