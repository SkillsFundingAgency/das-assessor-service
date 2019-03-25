using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.ViewModels.Organisation;
using FHADetails = SFA.DAS.AssessorService.ApplyTypes.FHADetails;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    public class OrganisationSearchController : Controller
    {
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IOrganisationsApplyApiClient _organisationsApplyApiClient;
        private readonly ILogger<OrganisationSearchController> _logger;
        private readonly IWebConfiguration _config;

        public OrganisationSearchController(ILogger<OrganisationSearchController> logger,
            IHttpContextAccessor contextAccessor, IOrganisationsApiClient organisationsApiClient,
            IContactsApiClient contactApiClient,
            IWebConfiguration config,
            IOrganisationsApplyApiClient organisationApplyApiClient)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _organisationsApiClient = organisationsApiClient;
            _contactsApiClient = contactApiClient;
            _config = config;
            _organisationsApplyApiClient = organisationApplyApiClient;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Results(OrganisationSearchViewModel viewModel)
        {
            var signinId = _contextAccessor.HttpContext.User.Claims.First(c => c.Type == "sub")?.Value;
            var user = await _contactsApiClient.GetContactBySignInId(signinId);

            if (!string.IsNullOrEmpty(user.EndPointAssessorOrganisationId) && user.OrganisationId != null &&
                user.Status == ContactStatus.Live)
                return RedirectToAction("Index", "Dashboard");

            if (string.IsNullOrEmpty(viewModel.SearchString) || viewModel.SearchString.Length < 2)
            {
                ModelState.AddModelError(nameof(viewModel.SearchString), "Enter a valid search string");
                TempData["ShowErrors"] = true;
                return View(nameof(Index));
            }

            var apiResponse = await _organisationsApplyApiClient.SearchForOrganisations(viewModel.SearchString);
            viewModel.Organisations = apiResponse?.GroupBy(r => r.Ukprn).Select(group => @group.First()).ToList();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> FromResults(OrganisationSearchViewModel viewModel)
        {
            var signinId = _contextAccessor.HttpContext.User.Claims.First(c => c.Type == "sub")?.Value;
            var user = await _contactsApiClient.GetContactBySignInId(signinId);

            if (!string.IsNullOrEmpty(user.EndPointAssessorOrganisationId) && user.OrganisationId != null &&
                user.Status == ContactStatus.Live)
                return RedirectToAction("Index", "Dashboard");

            if (string.IsNullOrEmpty(viewModel.SearchString) || viewModel.SearchString.Length < 2)
            {
                ModelState.AddModelError(nameof(viewModel.SearchString), "Enter a valid search string");
                TempData["ShowErrors"] = true;
                return View(nameof(Results), viewModel);
            }

            var apiResponse = await _organisationsApplyApiClient.SearchForOrganisations(viewModel.SearchString);
            viewModel.Organisations = apiResponse?.GroupBy(r => r.Ukprn).Select(group => @group.First()).ToList();
            return View(nameof(Results), viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(OrganisationSearchViewModel viewModel)
        {

            var signinId = _contextAccessor.HttpContext.User.Claims.First(c => c.Type == "sub")?.Value;
            var user = await _contactsApiClient.GetContactBySignInId(signinId);

            if (!string.IsNullOrEmpty(user.EndPointAssessorOrganisationId) && user.OrganisationId != null &&
                user.Status == ContactStatus.Live)
                return RedirectToAction("Index", "Dashboard");

            if (string.IsNullOrEmpty(viewModel.Name) || viewModel.SearchString.Length < 2)
            {
                ModelState.AddModelError(nameof(viewModel.Name), "Enter a valid search string");
                TempData["ShowErrors"] = true;
                return RedirectToAction(nameof(Index));
            }

            var organisationSearchResult = await GetOrganisation(viewModel.SearchString, viewModel.Name,
                viewModel.Ukprn, viewModel.OrganisationType, viewModel.Postcode);

            if (organisationSearchResult != null)
            {
                viewModel.Organisations = new List<OrganisationSearchResult> {organisationSearchResult};
                viewModel.OrganisationTypes = await _organisationsApiClient.GetOrganisationTypes();
            }

            return View(viewModel);

        }

        [HttpGet]
        public async Task<IActionResult> NoAccess(OrganisationSearchViewModel viewModel)
        {
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> OrganisationChosen(OrganisationSearchViewModel viewModel)
        {
            var organisationSearchResult = await GetOrganisation(viewModel.SearchString, viewModel.Name,
                viewModel.Ukprn, viewModel.OrganisationType, viewModel.Postcode);
            if (organisationSearchResult != null)
            {
                if (organisationSearchResult.OrganisationReferenceType == "RoEPAO")
                {
                    TempData["EpaoId"] = organisationSearchResult.OrganisationReferenceId;
                    return View(nameof(NoAccess), viewModel);
                }
                viewModel.Organisations = new List<OrganisationSearchResult> {organisationSearchResult};
                viewModel.OrganisationTypes = await _organisationsApiClient.GetOrganisationTypes();

                if (organisationSearchResult.OrganisationType == null)
                {
                    return View(nameof(Type), viewModel);
                }
            }
            return View(nameof(Confirm),viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> DealingWithRequest(OrganisationSearchViewModel viewModel)
        {
            var signinId = _contextAccessor.HttpContext.User.Claims.First(c => c.Type == "sub")?.Value;
            var user = await _contactsApiClient.GetContactBySignInId(signinId);

            if (!string.IsNullOrEmpty(user.EndPointAssessorOrganisationId) && user.OrganisationId != null &&
                user.Status == ContactStatus.Live)
                return RedirectToAction("Index", "Dashboard");

            var organisationSearchResult = await GetOrganisation(viewModel.SearchString, viewModel.Name,
                viewModel.Ukprn, viewModel.OrganisationType, viewModel.Postcode);
            if (organisationSearchResult != null)
            {
                if (organisationSearchResult.OrganisationReferenceType == "RoEPAO")
                {
                    var registeredOrganisation =
                        await _organisationsApiClient.Get(organisationSearchResult.Ukprn?.ToString());

                    await _contactsApiClient.UpdateOrgAndStatus(new UpdateContactWithOrgAndStausRequest(
                        user.Id.ToString(),
                        registeredOrganisation.Id.ToString(),
                        organisationSearchResult.Id,
                        ContactStatus.InvitePending));
                    await _organisationsApiClient.SendEmailsToOrgApprovedUsers(new EmailAllApprovedContactsRequest(
                        user.DisplayName, organisationSearchResult
                            .OrganisationReferenceId,  _config.ServiceLink));
                }
                else
                {
                    var orgDetails = new OrganisationDetails
                    {
                        OrganisationReferenceType = organisationSearchResult.OrganisationReferenceType,
                        OrganisationReferenceId = organisationSearchResult.OrganisationReferenceId,
                        LegalName = organisationSearchResult.LegalName,
                        TradingName = organisationSearchResult.TradingName,
                        ProviderName = organisationSearchResult.ProviderName,
                        CompanyNumber = organisationSearchResult.CompanyNumber,
                        CharityNumber = organisationSearchResult.CharityNumber,
                        Address1 = organisationSearchResult.Address?.Address1,
                        Address2 = organisationSearchResult.Address?.Address2,
                        Address3 = organisationSearchResult.Address?.Address3,
                        City = organisationSearchResult.Address?.City,
                        Postcode = organisationSearchResult.Address?.Postcode,
                        FHADetails = new FHADetails()
                        {
                            FinancialDueDate = organisationSearchResult.FinancialDueDate,
                            FinancialExempt = organisationSearchResult.FinancialExempt
                        }
                    };

                    var request = new ApplyTypes.CreateOrganisationRequest
                    {
                        Name = organisationSearchResult.Name,
                        OrganisationType = organisationSearchResult.OrganisationType,
                        OrganisationUkprn = organisationSearchResult.Ukprn,
                        RoEPAOApproved = organisationSearchResult.RoEPAOApproved,
                        RoATPApproved = organisationSearchResult.RoATPApproved,
                        OrganisationDetails = orgDetails,
                        CreatedBy = user.Id,
                        PrimaryContactEmail = organisationSearchResult.Email
                    };
                    await _contactsApiClient.UpdateStatus(new UpdateContactStatusRequest(user.Id.ToString(), ContactStatus.Applying));
                    await _organisationsApplyApiClient.ConfirmSearchedOrganisation(request);

                    return Redirect($"{_config.ApplyBaseAddress}/Applications");
                }
            }


            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Type(OrganisationSearchViewModel viewModel)
        {

            viewModel.OrganisationTypes = await _organisationsApiClient.GetOrganisationTypes();
            return View(viewModel);
        }
        

        private async Task<OrganisationSearchResult> GetOrganisation(string searchString, string name, int? ukprn,
            string organisationType, string postcode)
        {
            var searchResults = await _organisationsApplyApiClient.SearchForOrganisations(searchString);
            // filter ukprn
            searchResults = searchResults.Where(sr =>
                !sr.Ukprn.HasValue || !ukprn.HasValue || sr.Ukprn == ukprn);

            // filter name (identical match)
            searchResults = searchResults.Where(sr =>
                sr.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            // filter organisation type
            searchResults = searchResults.Where(sr =>
                sr.OrganisationType?.Equals(organisationType, StringComparison.InvariantCultureIgnoreCase) ?? true);

            // filter postcode
            searchResults = searchResults.Where(sr =>
                string.IsNullOrEmpty(postcode) ||
                (sr.Address?.Postcode.Equals(postcode, StringComparison.InvariantCultureIgnoreCase) ?? true));

            var organisationSearchResult = searchResults.FirstOrDefault();

            if (organisationSearchResult != null)
            {
                if (organisationSearchResult.OrganisationType == null)
                    organisationSearchResult.OrganisationType = organisationType;
            }

            return organisationSearchResult;
        }

    }
}