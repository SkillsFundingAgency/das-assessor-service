using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;
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
            var viewModel = MapViewOrganisationModel(organisation);
            await GatherOrganisationContacts(viewModel);

            var organisationStandards = await _apiClient.GetEpaOrganisationStandards(organisationId);
          

            foreach (var organisationStandard in organisationStandards)
            {
                var std = await _standardService.GetStandard(organisationStandard.StandardCode);
                organisationStandard.Standard = std;
            }

            //var allStandards = await _standardService.GetAllStandards();

            return View(viewModel);
        }

   
        private async Task GatherOrganisationContacts(RegisterViewOrganisationViewModel viewModel)
        {
            var contacts = await _apiClient.GetEpaOrganisationContacts(viewModel.OrganisationId);

            viewModel.Contacts = contacts;

            if (viewModel.PrimaryContact != null && contacts.Any(x => x.Username == viewModel.PrimaryContact))
            {
                var primaryContact = contacts.First(x => x.Username == viewModel.PrimaryContact);
                viewModel.PrimaryContactName = primaryContact.DisplayName;
                if (primaryContact.Username != null)
                {
                    viewModel.PrimaryContactName = $"{viewModel.PrimaryContactName} ({primaryContact.Username})";
                }
            }
        }
    
        private RegisterViewOrganisationViewModel MapViewOrganisationModel(EpaOrganisation organisation)
        {
            var notSetDescription = "Not Set";
            var viewModel = new RegisterViewOrganisationViewModel
            {
                OrganisationId = organisation.OrganisationId,
                Name = organisation.Name,
                Ukprn = organisation.Ukprn,
                OrganisationTypeId = organisation.OrganisationTypeId,
                OrganisationType = notSetDescription,
                LegalName = organisation.OrganisationData.LegalName,
                WebsiteLink = organisation.OrganisationData.WebsiteLink,
                Address1 = organisation.OrganisationData.Address1,
                Address2 = organisation.OrganisationData.Address2,
                Address3 = organisation.OrganisationData.Address3,
                Address4 = organisation.OrganisationData.Address4,
                Postcode = organisation.OrganisationData.Postcode,
                PrimaryContact = organisation.PrimaryContact,
                PrimaryContactName = notSetDescription
            };

            if (viewModel.OrganisationTypeId != null)
            {
                var organisationTypes = _apiClient.GetOrganisationTypes().Result;
                viewModel.OrganisationType = organisationTypes.First(x => x.Id == viewModel.OrganisationTypeId).Type;
            }

            return viewModel;
        }

    }
}
