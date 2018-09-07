using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize]
    public class RegisterController: Controller
    {
        public IActionResult Index()
        {
            return View(new RegisterViewModel());
        }

        [HttpGet("register/results")]
        public async Task<IActionResult> Results(string searchString)
        {
            //var searchResults = await _apiClient.Search(searchString, page);

            if (searchString.Trim().Length < 3)
            {
                var vm = new RegisterViewModel { ErrorMessage = "The expression entered is too short. Please enter 3 or more letters."};
                return View("index",vm);
            }
            var searchResults = new List<AssessmentOrganisationSummary>
            {
                new AssessmentOrganisationSummary {Id = "EPA0001", Name = "test", Ukprn = 1111111},
                new AssessmentOrganisationSummary {Id = "EPA0002", Name = "rest", Ukprn = 2222222}
            };

            var registerViewModel = new RegisterViewModel
            {
                Results = searchResults,
                SearchString = searchString,
            };

            return View(registerViewModel);
        }
    }
}
