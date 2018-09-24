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
            return View(new RegisterViewModel());
        }

        [HttpGet("register/results")]
        public async Task<IActionResult> Results(string searchString)
        {
          
            if (string.IsNullOrEmpty(searchString))
            {
                searchString = string.Empty;
            }
            else
            {
                searchString = searchString.Trim().ToLower();
            }

            if (searchString.Length < 2)
            {
                var vm = new RegisterViewModel { ErrorMessage = "The expression entered is too short. Please enter 2 or more letters."};
                return View("index",vm);
            }

            var searchResults = new List<AssessmentOrganisationSummary>();

            if (searchString == "test")
                searchResults = new List<AssessmentOrganisationSummary>
                {
                    new AssessmentOrganisationSummary {Id = "EPA0001", Name = "test", Ukprn = 1111111},
                    new AssessmentOrganisationSummary {Id = "EPA0002", Name = "rest", Ukprn = 2222222}
                };
            else
            {
                if (searchString != "empty")
                {
                    searchResults = await _apiClient.SearchOrganisations(searchString.Trim().ToLower());
                }
            }

            var registerViewModel = new RegisterViewModel
            {
                Results = searchResults,
                SearchString = searchString,
            };

            return View(registerViewModel);
        }
    }
}
