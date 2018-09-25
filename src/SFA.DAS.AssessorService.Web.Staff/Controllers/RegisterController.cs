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

            var searchResults = new List<AssessmentOrganisationSummary>();

            //MFCMFC present to help Greg develop front end
            if (vm.SearchString == "test")
                searchResults = new List<AssessmentOrganisationSummary>
                {
                    new AssessmentOrganisationSummary {Id = "EPA0001", Name = "test", Ukprn = 1111111},
                    new AssessmentOrganisationSummary {Id = "EPA0002", Name = "rest", Ukprn = 2222222}
                };
            else
            {
                if (vm.SearchString != "empty")
                {
                    searchResults = await _apiClient.SearchOrganisations(vm.SearchString?.Trim().ToLower());
                }
            }

            var registerViewModel = new RegisterViewModel
            {
                Results = searchResults,
                SearchString = vm.SearchString,
            };

            return View(registerViewModel);
        }
    }
}
