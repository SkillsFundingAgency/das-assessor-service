using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
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
        private readonly ISessionService _sessionService;

        public OrganisationSearchController(ILogger<OrganisationSearchController> logger,
            IHttpContextAccessor contextAccessor, IOrganisationsApiClient organisationsApiClient,
            IContactsApiClient contactApiClient,
            IWebConfiguration config,
            IOrganisationsApplyApiClient organisationApplyApiClient,
            ISessionService sessionService)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _organisationsApiClient = organisationsApiClient;
            _contactsApiClient = contactApiClient;
            _config = config;
            _organisationsApplyApiClient = organisationApplyApiClient;
            _sessionService = sessionService;
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

            viewModel.Organisations = await _organisationsApplyApiClient.SearchForOrganisations(viewModel.SearchString);
            viewModel.Organisations = OrderOrganisationByLiveStatus(viewModel);

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

            viewModel.Organisations = await _organisationsApplyApiClient.SearchForOrganisations(viewModel.SearchString);
            viewModel.Organisations = OrderOrganisationByLiveStatus(viewModel);

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

            if(string.IsNullOrEmpty(viewModel.OrganisationType))
            {
                ModelState.AddModelError(nameof(viewModel.OrganisationType), "Select an organisation type");
                TempData["ShowErrors"] = true;
                viewModel.OrganisationTypes = await _organisationsApiClient.GetOrganisationTypes();
                return View("Type", viewModel);
            }
           

            var organisationSearchResult = await GetOrganisation(viewModel.SearchString, viewModel.Name,
                viewModel.Ukprn, viewModel.OrganisationType, viewModel.Postcode);

            if (organisationSearchResult != null)
            {
                if (organisationSearchResult.CompanyNumber != null)
                {
                    var isActivelyTrading = await _organisationsApplyApiClient.IsCompanyActivelyTrading(organisationSearchResult.CompanyNumber);

                    if (!isActivelyTrading)
                    {
                        return View("~/Views/OrganisationSearch/CompanyNotActive.cshtml", viewModel);
                    }
                }

                viewModel.Organisations = new List<OrganisationSearchResult> {organisationSearchResult};
                viewModel.OrganisationTypes = await _organisationsApiClient.GetOrganisationTypes();
            }
            _sessionService.Set("OrganisationSearchViewModel", viewModel);
            return View(viewModel);

        }

        [HttpGet]
        public async Task<IActionResult> NoAccess(OrganisationSearchViewModel viewModel)
        {
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> OrganisationChosen()
        {
            var signinId = _contextAccessor.HttpContext.User.Claims.First(c => c.Type == "sub")?.Value;
            var user = await _contactsApiClient.GetContactBySignInId(signinId);
            var sessionString = _sessionService.Get("OrganisationSearchViewModel");
            if (sessionString == null)
            {
                _logger.LogInformation($"Session for OrganisationSearchViewModel requested by { user.DisplayName } has been lost. Redirecting to Search Index");
                return RedirectToAction("Index", "OrganisationSearch");
            }
            var viewModelFromSession = JsonConvert.DeserializeObject<OrganisationSearchViewModel>(sessionString);
            _sessionService.Remove("OrganisationSearchViewModel");

            var organisationSearchResult = await GetOrganisation(viewModelFromSession.SearchString, viewModelFromSession.Name,
                viewModelFromSession.Ukprn, viewModelFromSession.OrganisationType, viewModelFromSession.Postcode);
            viewModelFromSession.Organisations = new List<OrganisationSearchResult> { organisationSearchResult };
            viewModelFromSession.OrganisationTypes = await _organisationsApiClient.GetOrganisationTypes();

            return View("Type", viewModelFromSession);
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
                    return RequestAccess(viewModel, organisationSearchResult);
                }
                viewModel.Organisations = new List<OrganisationSearchResult> {organisationSearchResult};
                viewModel.OrganisationTypes = await _organisationsApiClient.GetOrganisationTypes();

                // ON-1818 do not pre-select OrganisationType
                // NOTE: ModelState overrides viewModel
                viewModel.OrganisationType = null;
                var orgTypeModelState = ModelState[nameof(viewModel.OrganisationType)];
                if (orgTypeModelState != null)
                {
                    orgTypeModelState.RawValue = viewModel.OrganisationType;
                    orgTypeModelState.Errors.Clear();
                }
                return View("Type", viewModel);
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

            var sessionString = _sessionService.Get("OrganisationSearchViewModel");
            if (sessionString != null)
                _sessionService.Remove("OrganisationSearchViewModel");

            var organisationSearchResult = await GetOrganisation(viewModel.SearchString, viewModel.Name,
                viewModel.Ukprn, viewModel.OrganisationType, viewModel.Postcode);
            if (organisationSearchResult != null)
            {
                if (organisationSearchResult.CompanyNumber != null)
                {
                    var isActivelyTrading = await _organisationsApplyApiClient.IsCompanyActivelyTrading(organisationSearchResult.CompanyNumber);

                    if (!isActivelyTrading)
                    {
                        return View("~/Views/OrganisationSearch/CompanyNotActive.cshtml", viewModel);
                    }
                }

                var request = CreateAnApplyOrganisationRequest(organisationSearchResult, user);
                if (organisationSearchResult.OrganisationReferenceType == "RoEPAO")
                {
                    //Update assessor organisation status, sync assessor org with apply org and notify org users
                    await UpdateOrganisationStatus(organisationSearchResult, user);
                    await TryToCreateOrganisationInApply(request);
                    await NotifyOrganisationUsers(organisationSearchResult, user);
                }
                else
                {
                    //Try creating a contact and an organisation in apply
                    await _contactsApiClient.MigrateSingleContactToApply(Guid.Parse(signinId));
                    await _contactsApiClient.UpdateStatus(new UpdateContactStatusRequest(user.Id.ToString(), ContactStatus.Applying));
                    await _organisationsApplyApiClient.ConfirmSearchedOrganisation(request);
                    return Redirect($"{_config.ApplyBaseAddress}/Applications");
                }
            }


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
            searchResults = searchResults.Where(sr => sr.RoATPApproved || 
                (sr.OrganisationType?.Equals(organisationType, StringComparison.InvariantCultureIgnoreCase) ?? true));

            // filter postcode
            searchResults = searchResults.Where(sr =>
                string.IsNullOrEmpty(postcode) ||
                (sr.Address?.Postcode.Equals(postcode, StringComparison.InvariantCultureIgnoreCase) ?? true));

            var organisationSearchResult = searchResults.FirstOrDefault();

            if (organisationSearchResult != null)
            {
                if (organisationSearchResult.RoATPApproved  || organisationSearchResult.OrganisationType == null)
                    organisationSearchResult.OrganisationType = organisationType;
            }

            return organisationSearchResult;
        }

        private List<OrganisationSearchResult> OrderOrganisationByLiveStatus(OrganisationSearchViewModel viewModel)
        {
            return viewModel.Organisations?.OrderByDescending(x => x.OrganisationIsLive).ToList();
        }

        private OrganisationDetails MapToOrganisationDetails(OrganisationSearchResult organisationSearchResult)
        {
           return new OrganisationDetails
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
        }

        private ApplyTypes.CreateOrganisationRequest CreateAnApplyOrganisationRequest(OrganisationSearchResult organisationSearchResult,
            ContactResponse user)
        {
            var orgDetails = MapToOrganisationDetails(organisationSearchResult);
            return  new ApplyTypes.CreateOrganisationRequest
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

        }

        private async Task UpdateOrganisationStatus(OrganisationSearchResult organisationSearchResult, ContactResponse user)
        {
            OrganisationResponse registeredOrganisation;

            if (organisationSearchResult.Ukprn != null)
                registeredOrganisation =
                    await _organisationsApiClient.Get(organisationSearchResult.Ukprn.ToString());
            else
            {
                var result =
                     await _organisationsApiClient.GetEpaOrganisation(organisationSearchResult.Id);
                registeredOrganisation = new OrganisationResponse
                {
                    Id = result.Id
                };
            }

            await _contactsApiClient.UpdateOrgAndStatus(new UpdateContactWithOrgAndStausRequest(
                     user.Id.ToString(),
                     registeredOrganisation.Id.ToString(),
                     organisationSearchResult.Id,
                     ContactStatus.InvitePending));
        }

        private async Task TryToCreateOrganisationInApply(ApplyTypes.CreateOrganisationRequest request)
        {
            try
            {
                //Check if orgnisation exists on apply
                await _organisationsApplyApiClient.DoesOrganisationExist(request.Name);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogInformation($"{e.Message}");
                //Try creating an organisation in apply
                await _organisationsApplyApiClient.CreateNewOrganisation(request);
            }
        }

        private async Task NotifyOrganisationUsers(OrganisationSearchResult organisationSearchResult,
          ContactResponse user)
        {
            await _organisationsApiClient.SendEmailsToOrgApprovedUsers(new EmailAllApprovedContactsRequest(
                       user.DisplayName, organisationSearchResult
                           .OrganisationReferenceId, _config.ServiceLink));
        }

        private ViewResult RequestAccess(OrganisationSearchViewModel viewModel, OrganisationSearchResult organisationSearchResult)
        {
            var newViewModel = Mapper.Map<RequestAccessOrgSearchViewModel>(viewModel);
            var addressArray = new[] { organisationSearchResult.Address?.Address1, organisationSearchResult.Address?.City, organisationSearchResult.Address.Postcode };
            newViewModel.Address = string.Join(", ", addressArray.Where(s => !string.IsNullOrEmpty(s)));
            newViewModel.RoEPAOApproved = organisationSearchResult.RoEPAOApproved;
            newViewModel.OrganisationIsLive = organisationSearchResult.OrganisationIsLive;

            if (!string.IsNullOrEmpty(organisationSearchResult.CompanyNumber))
            {
                newViewModel.CompanyOrCharityDisplayText = "Company number";
                newViewModel.CompanyNumber = organisationSearchResult.CompanyNumber;
            }
            else if (!string.IsNullOrEmpty(organisationSearchResult.CharityNumber))
            {
                newViewModel.CompanyOrCharityDisplayText = "Charity number";
                newViewModel.CompanyNumber = organisationSearchResult.CharityNumber;
            }
            return View(nameof(NoAccess), newViewModel);
        }
    }
}