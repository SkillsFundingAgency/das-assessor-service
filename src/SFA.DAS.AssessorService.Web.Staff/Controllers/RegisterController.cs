using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Interfaces;
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

        [HttpGet("register/add-organisation")]
        public async Task<IActionResult> AddOrganisation()
        {
            var vm = new RegisterAddOrganisationViewModel
            {
                OrganisationTypes = await _apiClient.GetOrganisationTypes()
            };

            return View(vm);
        }

        [HttpGet("register/add-contact/{organisationId}")]
        public async Task<IActionResult> AddContact(string organisationId)
        {
            var vm = new RegisterAddContactViewModel
            {
                EndPointAssessorOrganisationId = organisationId
            };

            return View(vm);
        }

        [HttpPost("register/add-contact/{organisationId}")]
        public async Task<IActionResult> AddContact(RegisterAddContactViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {      
                return View(viewModel);
            }

            var addContactRequest = new CreateEpaOrganisationContactRequest
            {
                EndPointAssessorOrganisationId = viewModel.EndPointAssessorOrganisationId,
                DisplayName =  viewModel.DisplayName,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber
                
            };

            var contactId = await _apiClient.CreateEpaContact(addContactRequest);
            return Redirect($"/register/view-contact/{contactId}");
            
        }

        [HttpGet("register/view-contact/{contactId}")]
        public async Task<IActionResult> ViewContact(string contactId)
        {
            var viewContact = new RegisterViewContactViewModel { ContactId = contactId };
            return View(viewContact);
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
            
        [HttpPost("register/add-organisation")]
        public async Task<IActionResult> AddOrganisation(RegisterAddOrganisationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.OrganisationTypes = await _apiClient.GetOrganisationTypes();             
                return View(viewModel);
            }

            var addOrganisationRequest = new CreateEpaOrganisationRequest
            {
                Name = viewModel.Name,
                Ukprn = viewModel.Ukprn,
                OrganisationTypeId = viewModel.OrganisationTypeId,
                LegalName = viewModel.LegalName,
                WebsiteLink = viewModel.WebsiteLink,
                Address1 = viewModel.Address1,
                Address2 = viewModel.Address2,
                Address3 = viewModel.Address3,
                Address4 = viewModel.Address4,
                Postcode = viewModel.Postcode
            };

            var organisationId = await _apiClient.CreateEpaOrganisation(addOrganisationRequest);
            return Redirect($"view-organisation/{organisationId}");
        }

        [HttpGet("register/view-organisation/{organisationId}")]
        public async Task<IActionResult> ViewOrganisation(string organisationId)
        {
            var organisation = await _apiClient.GetEpaOrganisation(organisationId);
            var viewOrganisation = new RegisterViewOrganisationViewModel { OrganisationId = organisation.OrganisationId };
            return View(viewOrganisation);
        }
    }
}
