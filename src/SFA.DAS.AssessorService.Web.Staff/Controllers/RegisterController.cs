using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.Services;
using SFA.DAS.AssessorService.Web.Staff.Domain;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam + "," + Roles.RegisterViewOnlyTeam)]
    public class RegisterController: Controller
    {
        private readonly ApiClient _apiClient;
        private readonly IStandardService _standardService;
        private readonly IHostingEnvironment _env;
        public RegisterController(ApiClient apiClient, IStandardService standardService,  IHostingEnvironment env)
        {
            _apiClient = apiClient;
            _standardService = standardService;
            _env = env;
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


        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/edit-organisation/{organisationId}")]
        public async Task<IActionResult> EditOrganisation(string organisationId)
        {
            var organisation = await _apiClient.GetEpaOrganisation(organisationId);
            var viewModel = MapOrganisationModel(organisation);
            return View(viewModel);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
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
                TradingName = viewModel.TradingName,
                WebsiteLink = viewModel.WebsiteLink,
                Address1 = viewModel.Address1,
                Address2 = viewModel.Address2,
                Address3 = viewModel.Address3,
                Address4 = viewModel.Address4,
                Postcode = viewModel.Postcode,
                Status = viewModel.Status,
                ActionChoice = viewModel.ActionChoice,
                CompanyNumber = viewModel.CompanyNumber,
                CharityNumber = viewModel.CharityNumber,
                FinancialDueDate = viewModel.FinancialDueDate,
                FinancialExempt = viewModel.FinancialExempt
            };
         
            await _apiClient.UpdateEpaOrganisation(updateOrganisationRequest);
         
            return RedirectToAction("ViewOrganisation", "register", new { organisationId = viewModel.OrganisationId});
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/add-organisation")]
        public async Task<IActionResult> AddOrganisation()
        {
            var vm = new RegisterOrganisationViewModel
            {
                OrganisationTypes = await _apiClient.GetOrganisationTypes()
            };

            return View(vm);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/add-standard/organisation/{organisationId}/standard/{standardId}")]
        public async Task<IActionResult> AddOrganisationStandard(string organisationId, int standardId)
       {
           var viewModelToHydrate =
               new RegisterAddOrganisationStandardViewModel {OrganisationId = organisationId, StandardId = standardId};
           var vm = await ConstructOrganisationAndStandardDetails(viewModelToHydrate);

           return View(vm);
       }


        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpPost("register/add-standard/organisation/{organisationId}/standard/{standardId}")]
        public async Task<IActionResult> AddOrganisationStandard(RegisterAddOrganisationStandardViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var viewModelInvalid = await ConstructOrganisationAndStandardDetails(viewModel);
                return View(viewModelInvalid);
            }                   

            var addOrganisationStandardRequest = new CreateEpaOrganisationStandardRequest
            {
                OrganisationId = viewModel.OrganisationId,
               StandardCode = viewModel.StandardId,
               EffectiveFrom = viewModel.EffectiveFrom,
               EffectiveTo = viewModel.EffectiveTo,
               ContactId = viewModel.ContactId.ToString(),
               DeliveryAreas = viewModel.DeliveryAreas,
               Comments = viewModel.Comments,
               DeliveryAreasComments = viewModel.DeliveryAreasComments
            };

            var organisationStandardId = await _apiClient.CreateEpaOrganisationStandard(addOrganisationStandardRequest);
            return Redirect($"/register/view-standard/{organisationStandardId}");
        }

        [HttpGet("register/view-standard/{organisationStandardId}")]
        public async Task<IActionResult> ViewStandard(int organisationStandardId)
        {
            var organisationStandard = await _apiClient.GetOrganisationStandard(organisationStandardId);


            var viewModel =
                MapOrganisationStandardToViewModel(organisationStandard);

            return View(viewModel);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/edit-standard/{organisationStandardId}")]
        public async Task<IActionResult> EditOrganisationStandard(int organisationStandardId)
        {
            var organisationStandard = await _apiClient.GetOrganisationStandard(organisationStandardId);
            var viewModel =
                MapOrganisationStandardToViewModel(organisationStandard);
            var vm = await AddContactsAndDeliveryAreasAndDateDetails(viewModel);
            return View(vm);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpPost("register/edit-standard/{organisationStandardId}")]
        public async Task<IActionResult> EditOrganisationStandard(RegisterViewAndEditOrganisationStandardViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var viewModelInvalid = await AddContactsAndDeliveryAreasAndDateDetails(viewModel);
                return View(viewModelInvalid);
            }

            var updateOrganisationStandardRequest = new UpdateEpaOrganisationStandardRequest
            {
                OrganisationId = viewModel.OrganisationId,
                StandardCode = viewModel.StandardId,
                EffectiveFrom = viewModel.EffectiveFrom,
                EffectiveTo = viewModel.EffectiveTo,
                ContactId = viewModel.ContactId.ToString(),
                DeliveryAreas = viewModel.DeliveryAreas,
                Comments = viewModel.Comments,
                OrganisationStatus = viewModel.OrganisationStatus,
                OrganisationStandardStatus = viewModel.Status,
                ActionChoice = viewModel.ActionChoice,
                DeliveryAreasComments = viewModel.DeliveryAreasComments
            };

            var organisationStandardId = await _apiClient.UpdateEpaOrganisationStandard(updateOrganisationStandardRequest);
            return Redirect($"/register/view-standard/{organisationStandardId}");
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/add-contact/{organisationId}")]
        public async Task<IActionResult> AddContact(string organisationId)
        {
            var vm = new RegisterAddContactViewModel
            {
                EndPointAssessorOrganisationId = organisationId
            };

            return View(vm);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
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

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/edit-contact/{contactId}")]
        public async Task<IActionResult> EditContact(string contactId)
        {
            var contact = await _apiClient.GetEpaCntact(contactId);
            var organisation = await _apiClient.GetEpaOrganisation(contact.OrganisationId);
            var viewModel = MapContactModel(contact, organisation);
            return View(viewModel);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
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
                PhoneNumber = viewAndEditModel.PhoneNumber,
                ActionChoice = viewAndEditModel.ActionChoice
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
            if (!_env.IsDevelopment())
                return NotFound();
           
            var vm = new AssessmentOrgsImportResponse { Status = "Press to run" };         
            return View(vm);
        }
        [HttpGet("register/impage-{choice}")]
        public async Task<IActionResult> Impage(string choice)
        {
            if (!_env.IsDevelopment())
                return NotFound();

            var vm = new AssessmentOrgsImportResponse { Status = "Running" };
            if (choice == "DoIt")
            {
                var importResults = await _apiClient.ImportOrganisations();
                vm.Status = importResults;
            }
            return View(vm);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
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
                TradingName = viewModel.TradingName,
                WebsiteLink = viewModel.WebsiteLink,
                Address1 = viewModel.Address1,
                Address2 = viewModel.Address2,
                Address3 = viewModel.Address3,
                Address4 = viewModel.Address4,
                Postcode = viewModel.Postcode,
                CompanyNumber = viewModel.CompanyNumber,
                CharityNumber = viewModel.CharityNumber
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


        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/search-standards/{organisationId}")]
        public async Task<IActionResult> SearchStandards(string organisationId)
        {
            var organisation = await _apiClient.GetEpaOrganisation(organisationId);
            var vm = new SearchStandardsViewModel{OrganisationId = organisationId, OrganisationName = organisation.Name};
            
            return View(vm);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
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

        private async Task<RegisterAddOrganisationStandardViewModel> ConstructOrganisationAndStandardDetails(RegisterAddOrganisationStandardViewModel vm)
        {
            var organisation = await _apiClient.GetEpaOrganisation(vm.OrganisationId);
            var standard = await _standardService.GetStandard(vm.StandardId);
            var availableDeliveryAreas = await _apiClient.GetDeliveryAreas();

            vm.Contacts = await _apiClient.GetEpaOrganisationContacts(vm.OrganisationId);
            vm.OrganisationName = organisation.Name;
            vm.Ukprn = organisation.Ukprn;
            vm.StandardTitle = standard.Title;
            vm.StandardEffectiveFrom = standard.EffectiveFrom;
            vm.StandardEffectiveTo = standard.EffectiveTo;
            vm.StandardLastDateForNewStarts = standard.LastDateForNewStarts;
            vm.AvailableDeliveryAreas = availableDeliveryAreas;
            vm.DeliveryAreas = vm.DeliveryAreas ?? new List<int>();
            vm.OrganisationStatus = organisation.Status;
            return vm;
        }


        private async Task<RegisterViewAndEditOrganisationStandardViewModel> AddContactsAndDeliveryAreasAndDateDetails(RegisterViewAndEditOrganisationStandardViewModel vm)
        {
            var availableDeliveryAreas = await _apiClient.GetDeliveryAreas();

            vm.Contacts = await _apiClient.GetEpaOrganisationContacts(vm.OrganisationId);
            vm.AvailableDeliveryAreas = availableDeliveryAreas;
            vm.DeliveryAreas = vm.DeliveryAreas ?? new List<int>();
            if (vm.EffectiveFrom.HasValue)
            {
                var effectiveFrom = vm.EffectiveFrom.Value;
                vm.EffectiveFromDay = effectiveFrom.Day.ToString();
                vm.EffectiveFromMonth = effectiveFrom.Month.ToString();
                vm.EffectiveFromYear = effectiveFrom.Year.ToString();
            }

            if (vm.EffectiveTo.HasValue)
            {
                var effectiveTo = vm.EffectiveTo.Value;
                vm.EffectiveToDay = effectiveTo.Day.ToString();
                vm.EffectiveToMonth = effectiveTo.Month.ToString();
                vm.EffectiveToYear = effectiveTo.Year.ToString();
            }

            return vm;
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
                OrganisationId = organisation.OrganisationId,
                IsPrimaryContact = contact.IsPrimaryContact
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
                TradingName = organisation.OrganisationData?.TradingName,
                WebsiteLink = organisation.OrganisationData?.WebsiteLink,
                Address1 = organisation.OrganisationData?.Address1,
                Address2 = organisation.OrganisationData?.Address2,
                Address3 = organisation.OrganisationData?.Address3,
                Address4 = organisation.OrganisationData?.Address4,
                Postcode = organisation.OrganisationData?.Postcode,
                PrimaryContact = organisation.PrimaryContact,
                PrimaryContactName = notSetDescription,
                CharityNumber = organisation.OrganisationData?.CharityNumber,
                CompanyNumber =  organisation.OrganisationData?.CompanyNumber,
                Status = organisation.Status,
                FinancialDueDate = organisation.OrganisationData?.FHADetails?.FinancialDueDate,
                FinancialExempt = organisation.OrganisationData?.FHADetails?.FinancialExempt
            };

            viewModel.OrganisationTypes = _apiClient.GetOrganisationTypes().Result;

            if (viewModel.OrganisationTypeId != null)
            {
                var organisationTypes = viewModel.OrganisationTypes;
                viewModel.OrganisationType = organisationTypes.FirstOrDefault(x => x.Id == viewModel.OrganisationTypeId)?.Type;
            }
               
            GatherOrganisationContacts(viewModel);
            GatherOrganisationStandards(viewModel);

            return viewModel;
        }

        private static RegisterViewAndEditOrganisationStandardViewModel MapOrganisationStandardToViewModel(OrganisationStandard organisationStandard)
        {
            return new RegisterViewAndEditOrganisationStandardViewModel
            {
                OrganisationStandardId = organisationStandard.Id,
                StandardId = organisationStandard.StandardId,
                StandardTitle = organisationStandard.StandardTitle,
                OrganisationId = organisationStandard.OrganisationId,
                Ukprn = organisationStandard.Ukprn,
                EffectiveFrom = organisationStandard.EffectiveFrom,
                EffectiveTo = organisationStandard.EffectiveTo,
                DateStandardApprovedOnRegister = organisationStandard.DateStandardApprovedOnRegister,
                StandardEffectiveFrom = organisationStandard.StandardEffectiveFrom,
                StandardEffectiveTo = organisationStandard.StandardEffectiveTo,
                StandardLastDateForNewStarts = organisationStandard.StandardLastDateForNewStarts,
                Comments = organisationStandard.Comments,
                Status = organisationStandard.Status,
                ContactId = organisationStandard.ContactId,
                Contact = organisationStandard.Contact,
                DeliveryAreas = organisationStandard.DeliveryAreas,
                OrganisationName = organisationStandard.OrganisationName,
                OrganisationStatus = organisationStandard.OrganisationStatus,
                DeliveryAreasDetails = organisationStandard.DeliveryAreasDetails,
                DeliveryAreasComments = organisationStandard.OrganisationStandardData?.DeliveryAreasComments
            };
        }
    }
}
