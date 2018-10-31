using System.Collections.Generic;
using System.Linq;
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
using SFA.DAS.AssessorService.Web.Staff.Services;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize]
    public class RegisterController: Controller
    {
        private readonly ApiClient _apiClient;
        private readonly IStandardService _standardService;

        public RegisterController(ApiClient apiClient, IStandardService standardService)
        {
            _apiClient = apiClient;
            _standardService = standardService;
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

            var results = searchResults ?? new List<AssessmentOrganisationSummary>();
            var registerViewModel = new RegisterViewModel
            {
                Results = results,
                SearchString = vm.SearchString
            };

            return View(registerViewModel);
        }


        [HttpGet("register/edit-organisation/{organisationId}")]
        public async Task<IActionResult> EditOrganisation(string organisationId)
        {
            var organisation = await _apiClient.GetEpaOrganisation(organisationId);
            var viewModel = MapOrganisationModel(organisation);
            return View(viewModel);
        }


        [HttpPost("register/edit-organisation/{organisationId}")]
                 public async Task<IActionResult> EditOrganisation(RegisterViewAndEditOrganisationViewModel viewModel)
                 {
                     if (!ModelState.IsValid)
                     {
                         viewModel.OrganisationTypes = await _apiClient.GetOrganisationTypes();
                         GatherOrganisationContacts(viewModel);
                         GatherOrganisationStandards(viewModel);
                         return View(viewModel);
                     }
         
                     var updateOrganisationRequest = new UpdateEpaOrganisationRequest
                     {
                         Name = viewModel.Name,
                         OrganisationId = viewModel.OrganisationId,
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
         
                     await _apiClient.UpdateEpaOrganisation(updateOrganisationRequest);
         
                     return RedirectToAction("ViewOrganisation", "register", new { organisationId = viewModel.OrganisationId});
                 }

        [HttpGet("register/add-organisation")]
        public async Task<IActionResult> AddOrganisation()
        {
            var vm = new RegisterOrganisationViewModel
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

        [HttpGet("register/edit-contact/{contactId}")]
        public async Task<IActionResult> EditContact(string contactId)
        {
            var contact = await _apiClient.GetEpaCntact(contactId);
            var organisation = await _apiClient.GetEpaOrganisation(contact.OrganisationId);
            var viewModel = MapContactModel(contact, organisation);
            return View(viewModel);
        }

        [HttpPost("register/edit-contact/{contactId}")]
        public async Task<IActionResult> EditContact(RegisterViewAndEditContactViewModel viewAndEditModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewAndEditModel);
            }

            var request = new UpdateEpaOrganisationContactRequest
            {
                ContactId = viewAndEditModel.ContactId,
                DisplayName =  viewAndEditModel.DisplayName,
                Email = viewAndEditModel.Email,
                PhoneNumber = viewAndEditModel.PhoneNumber 
            };
            await _apiClient.UpdateEpaContact(request);
            return RedirectToAction("ViewContact", "register", new { contactId = viewAndEditModel.ContactId});
        }
        
        [HttpGet("register/view-contact/{contactId}")]
        public async Task<IActionResult> ViewContact(string contactId)
        {
            var contact = await _apiClient.GetEpaCntact(contactId);
            var organisation = await _apiClient.GetEpaOrganisation(contact.OrganisationId);
            var viewModel = MapContactModel(contact, organisation);
            return View(viewModel);
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
        public async Task<IActionResult> AddOrganisation(RegisterOrganisationViewModel viewModel)
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
            return RedirectToAction("ViewOrganisation", "register",new { organisationId });
        }

        [HttpGet("register/view-organisation/{organisationId}")]
        public async Task<IActionResult> ViewOrganisation(string organisationId)
        {    
            var organisation = await _apiClient.GetEpaOrganisation(organisationId);
            var viewModel = MapOrganisationModel(organisation);     
            return View(viewModel);
        }


        [HttpGet("register/search-standards/{organisationId}")]
        public async Task<IActionResult> SearchStandards(string organisationId)
        {
            var organisation = await _apiClient.GetEpaOrganisation(organisationId);
            var vm = new SearchStandardsViewModel{OrganisationId = organisationId, OrganisationName = organisation.Name};
            
            return View(vm);
        }

        [HttpGet("register/search-standards-results")]
        public async Task<IActionResult> SearchStandardsResults(SearchStandardsViewModel vm)
        {
            var organisation = await _apiClient.GetEpaOrganisation(vm.OrganisationId);
            vm.OrganisationName = organisation.Name;
            if (!ModelState.IsValid)
            {
                return View("SearchStandards", vm);
            }

            var searchstring = vm.StandardSearchString?.Trim().ToLower();
            searchstring = string.IsNullOrEmpty(searchstring) ? "" : searchstring;
            var rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
            searchstring = rx.Replace(searchstring, "");
            searchstring = searchstring.Replace("/", "");
            var searchResults = await _apiClient.SearchStandards(searchstring);

            var standardViewModel = new SearchStandardsViewModel
            {
                Results = searchResults,
                StandardSearchString = vm.StandardSearchString,
                OrganisationId = vm.OrganisationId,
                OrganisationName = vm.OrganisationName
            };

            return View(standardViewModel);
        }


        private void GatherOrganisationStandards(RegisterViewAndEditOrganisationViewModel viewAndEditModel)
        {
            var organisationStandards = _apiClient.GetEpaOrganisationStandards(viewAndEditModel.OrganisationId).Result;

            var allStandards = _standardService.GetAllStandardSummaries().Result;

            foreach (var organisationStandard in organisationStandards)
            {
                var std = allStandards.First(x => x.Id == organisationStandard.StandardCode.ToString());
                organisationStandard.StandardSummary = std;
            }

            viewAndEditModel.OrganisationStandards = organisationStandards;
        }

        private RegisterViewAndEditContactViewModel MapContactModel(AssessmentOrganisationContact contact, EpaOrganisation organisation)
        {
            var viewModel = new RegisterViewAndEditContactViewModel
            {
                Email = contact.Email,
                ContactId = contact.Id.ToString(),
                PhoneNumber = contact.PhoneNumber,
                DisplayName = contact.DisplayName,
                OrganisationName = organisation.Name,
                OrganisationId = organisation.OrganisationId
            };

            return viewModel;
        }

        private void GatherOrganisationContacts(RegisterViewAndEditOrganisationViewModel viewAndEditModel)
        {
            var contacts =  _apiClient.GetEpaOrganisationContacts(viewAndEditModel.OrganisationId).Result;

            viewAndEditModel.Contacts = contacts;

            if (viewAndEditModel.PrimaryContact != null && contacts.Any(x => x.Username == viewAndEditModel.PrimaryContact))
            {
                var primaryContact = contacts.First(x => x.Username == viewAndEditModel.PrimaryContact);
                viewAndEditModel.PrimaryContactName = primaryContact.DisplayName;
                if (primaryContact.Username != null)
                {
                    viewAndEditModel.PrimaryContactName = $"{viewAndEditModel.PrimaryContactName} ({primaryContact.Username})";
                }
            }
        }
    
        private RegisterViewAndEditOrganisationViewModel MapOrganisationModel(EpaOrganisation organisation)
        {
            var notSetDescription = "Not set";
            var viewModel = new RegisterViewAndEditOrganisationViewModel
            {
                OrganisationId = organisation.OrganisationId,
                Name = organisation.Name,
                Ukprn = organisation.Ukprn,
                OrganisationTypeId = organisation.OrganisationTypeId,
                OrganisationType = notSetDescription,
                LegalName = organisation.OrganisationData?.LegalName,
                WebsiteLink = organisation.OrganisationData?.WebsiteLink,
                Address1 = organisation.OrganisationData?.Address1,
                Address2 = organisation.OrganisationData?.Address2,
                Address3 = organisation.OrganisationData?.Address3,
                Address4 = organisation.OrganisationData?.Address4,
                Postcode = organisation.OrganisationData?.Postcode,
                PrimaryContact = organisation.PrimaryContact,
                PrimaryContactName = notSetDescription,
                Status = organisation.Status
            };

            if (viewModel.OrganisationTypeId != null)
            {
                var organisationTypes = _apiClient.GetOrganisationTypes().Result;
                viewModel.OrganisationType = organisationTypes.First(x => x.Id == viewModel.OrganisationTypeId).Type;
            }
            viewModel.OrganisationTypes = _apiClient.GetOrganisationTypes().Result;
            
            GatherOrganisationContacts(viewModel);
            GatherOrganisationStandards(viewModel);

            return viewModel;
        }
    }
}
