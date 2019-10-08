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
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Organisation;

namespace SFA.DAS.AssessorService.Web.Controllers
{

    [Authorize]
    public class OrganisationSearchController : Controller
    {
        private const int PageSize = 10;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly ILogger<OrganisationSearchController> _logger;
        private readonly IWebConfiguration _config;
        private readonly ISessionService _sessionService;

        public OrganisationSearchController(ILogger<OrganisationSearchController> logger,
            IHttpContextAccessor contextAccessor, IOrganisationsApiClient organisationsApiClient,
            IContactsApiClient contactApiClient,
            IWebConfiguration config,
            ISessionService sessionService)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _organisationsApiClient = organisationsApiClient;
            _contactsApiClient = contactApiClient;
            _config = config;
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Results(OrganisationSearchViewModel viewModel, int? pageIndex)
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

            viewModel.Organisations = await _organisationsApiClient.SearchForOrganisations(viewModel.SearchString, PageSize, SanitizePageIndex(pageIndex));

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> FromResults(OrganisationSearchViewModel viewModel, int? pageIndex)
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

            viewModel.Organisations = await _organisationsApiClient.SearchForOrganisations(viewModel.SearchString, PageSize, SanitizePageIndex(pageIndex));

            return View(nameof(Results), viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> NextResults(string searchString, int? pageIndex)
        {
            var viewModel = new OrganisationSearchViewModel
            {
                SearchString = searchString
            };

            if (string.IsNullOrEmpty(viewModel.SearchString) || viewModel.SearchString.Length < 2)
            {
                ModelState.AddModelError(nameof(viewModel.SearchString), "Enter a valid search string");
                TempData["ShowErrors"] = true;
                return View(nameof(Results), viewModel);
            }
            
            viewModel.Organisations = await _organisationsApiClient.SearchForOrganisations(viewModel.SearchString, PageSize, SanitizePageIndex(pageIndex));

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

            if(!string.IsNullOrEmpty(viewModel.OrganisationType))
            {
                viewModel.OrganisationTypes = await _organisationsApiClient.GetOrganisationTypes();
                viewModel.OrganisationTypeId = viewModel.OrganisationTypes.Any()? viewModel.OrganisationTypes
                    .First(x => viewModel.OrganisationType != null && x.Type == viewModel.OrganisationType).Id:0;
            }
           

            var organisationSearchResult = await GetOrganisation(viewModel.SearchString, viewModel.Name,
                viewModel.Ukprn, viewModel.OrganisationType, viewModel.Postcode, viewModel.PageIndex);

            if (organisationSearchResult != null)
            {
                if (organisationSearchResult.CompanyNumber != null)
                {
                    var isActivelyTrading = await _organisationsApiClient.IsCompanyActivelyTrading(organisationSearchResult.CompanyNumber);

                    if (!isActivelyTrading)
                    {
                        return View("~/Views/OrganisationSearch/CompanyNotActive.cshtml", viewModel);
                    }
                }

                viewModel.Organisations = new PaginatedList<OrganisationSearchResult>(new List<OrganisationSearchResult> { organisationSearchResult },1,1,1);
                viewModel.OrganisationTypes = await _organisationsApiClient.GetOrganisationTypes();
            }
            _sessionService.Set("OrganisationSearchViewModel", viewModel);
            return View(viewModel);

        }

        [HttpGet]
        public IActionResult NoAccess(OrganisationSearchViewModel viewModel)
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
                viewModelFromSession.Ukprn, viewModelFromSession.OrganisationType, viewModelFromSession.Postcode,null);
            viewModelFromSession.Organisations = new PaginatedList<OrganisationSearchResult>(new List<OrganisationSearchResult> { organisationSearchResult }, 1, 1, PageSize);
            viewModelFromSession.OrganisationTypes = await _organisationsApiClient.GetOrganisationTypes();

            return View("Type", viewModelFromSession);
        }

        [HttpPost]
        public async Task<IActionResult> OrganisationChosen(OrganisationSearchViewModel viewModel)
        {
            var organisationSearchResult = await GetOrganisation(viewModel.SearchString, viewModel.Name,
                viewModel.Ukprn, viewModel.OrganisationType, viewModel.Postcode, viewModel.PageIndex);
            if (organisationSearchResult != null)
            {
                if (organisationSearchResult.OrganisationReferenceType == "RoEPAO")
                {
                    return RequestAccess(viewModel, organisationSearchResult);
                }
                viewModel.Organisations = new PaginatedList<OrganisationSearchResult>(new List<OrganisationSearchResult> { organisationSearchResult }, 1, 1, PageSize);
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

            // Why would a new user searching for an Organisation have an EPAOrgId or an OrganisationId?
            if (!string.IsNullOrEmpty(user.EndPointAssessorOrganisationId) && user.OrganisationId != null &&
                user.Status == ContactStatus.Live)
                return RedirectToAction("Index", "Dashboard");

            var sessionString = _sessionService.Get("OrganisationSearchViewModel");
            if (sessionString != null)
                _sessionService.Remove("OrganisationSearchViewModel");

            var organisationSearchResult = await GetOrganisation(viewModel.SearchString, viewModel.Name,
                viewModel.Ukprn, viewModel.OrganisationType, viewModel.Postcode, viewModel.PageIndex);
            if (organisationSearchResult != null)
            { 
                if (organisationSearchResult.CompanyNumber != null)
                {
                    var isActivelyTrading = await _organisationsApiClient.IsCompanyActivelyTrading(organisationSearchResult.CompanyNumber);

                    if (!isActivelyTrading)
                    {
                        return View("~/Views/OrganisationSearch/CompanyNotActive.cshtml", viewModel);
                    }
                }

                if (organisationSearchResult.OrganisationReferenceType == "RoEPAO")
                {
                    if (organisationSearchResult.OrganisationIsLive)
                        //Update assessor organisation status
                        await UpdateOrganisationStatusAndInvite(organisationSearchResult, user);
                }
                else
                {
                    var request = await CreateEpaOrganisationRequest(organisationSearchResult);
                    request.OrganisationTypeId = viewModel.OrganisationTypeId;
                    request.Status = OrganisationStatus.Applying;

                    var epaoId = await _organisationsApiClient.CreateEpaOrganisation(request);
                    _logger.LogInformation($"Organisation with Organisation Id {epaoId.Details} created.");

                    var newOrg = await _organisationsApiClient.GetEpaOrganisation(epaoId.Details);
                    var response = await _contactsApiClient.UpdateOrgAndStatus(new UpdateContactWithOrgAndStausRequest(user.Id.ToString(),
                        newOrg.Id.ToString(), null, ContactStatus.Live));
                    _logger.LogInformation($"Contact with display name {user.DisplayName} is associated with organisation {epaoId.Details}.");

                    _sessionService.Set("OrganisationName", newOrg.Name);
                    
                    return RedirectToAction("Applications", "Application");
                }
            }
            return View(viewModel);
        }  
     
        private async Task<OrganisationSearchResult> GetOrganisation(string searchString, string name, int? ukprn,
            string organisationType, string postcode, int? pageIndex)
        {
            var searchResultsReturned = await _organisationsApiClient.SearchForOrganisations(searchString,PageSize, SanitizePageIndex(pageIndex));
            var searchResults = searchResultsReturned.Items == null ? 
                new List<OrganisationSearchResult>().AsEnumerable() : searchResultsReturned.Items.AsEnumerable();

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

        private async Task<CreateEpaOrganisationRequest> CreateEpaOrganisationRequest(OrganisationSearchResult organisationSearchResult)
        {
            // ON-2262 - Get the company & charity details from the relevant APIs
            var companyDetails = !string.IsNullOrWhiteSpace(organisationSearchResult.CompanyNumber) ? await _organisationsApiClient.GetCompanyDetails(organisationSearchResult.CompanyNumber) : null;
            var charityDetails = int.TryParse(organisationSearchResult.CompanyNumber, out var charityNumber) ? await _organisationsApiClient.GetCharityDetails(charityNumber) : null;

            return new CreateEpaOrganisationRequest
            {
                Name = organisationSearchResult.Name,
                OrganisationReferenceType = organisationSearchResult.OrganisationReferenceType,
                OrganisationReferenceId = organisationSearchResult.OrganisationReferenceId,
                LegalName = organisationSearchResult.LegalName,
                TradingName = organisationSearchResult.TradingName,
                ProviderName = organisationSearchResult.ProviderName,
                CompanyNumber = organisationSearchResult.CompanyNumber,
                CompanySummary = Mapper.Map<CompaniesHouseSummary>(companyDetails),
                CharityNumber = organisationSearchResult.CharityNumber,
                CharitySummary = Mapper.Map<CharityCommissionSummary>(charityDetails),
                Address1 = organisationSearchResult.Address?.Address1,
                Address2 = organisationSearchResult.Address?.Address2,
                Address3 = organisationSearchResult.Address?.Address3,
                City = organisationSearchResult.Address?.City,
                Postcode = organisationSearchResult.Address?.Postcode,
                RoATPApproved =  organisationSearchResult.RoATPApproved,
                RoEPAOApproved= false,
                EndPointAssessmentOrgId = null,
                FHADetails = new Api.Types.Models.AO.FHADetails
                {
                    FinancialDueDate = organisationSearchResult.FinancialDueDate,
                    FinancialExempt = organisationSearchResult.FinancialExempt
                }
            };
        }

        private async Task UpdateOrganisationStatusAndInvite(OrganisationSearchResult organisationSearchResult, ContactResponse user)
        {
            var registeredOrganisation = await RetrieveOrganisation(organisationSearchResult);
            if (registeredOrganisation.Status == OrganisationStatus.Live || registeredOrganisation.Status == OrganisationStatus.New)
            {
                await _contactsApiClient.UpdateOrgAndStatus(new UpdateContactWithOrgAndStausRequest(
                    user.Id.ToString(),
                    registeredOrganisation?.Id.ToString(),
                    organisationSearchResult.Id,
                    ContactStatus.InvitePending));

                await NotifyOrganisationUsers(organisationSearchResult, user);
            }
        }


        private async Task<OrganisationResponse> RetrieveOrganisation(OrganisationSearchResult organisationSearchResult)
        {
            var result =
                await _organisationsApiClient.GetEpaOrganisation(organisationSearchResult.Id);
            var registeredOrganisation = new OrganisationResponse
            {
                Id = result.Id
            };
            return registeredOrganisation;
        }

        private async Task NotifyOrganisationUsers(OrganisationSearchResult organisationSearchResult,
          ContactResponse user)
        {
            //ON-2020 Changed from reference Id to Id , since org reference Id can have multiple values 
            await _organisationsApiClient.SendEmailsToOrganisationUserManagementUsers(new NotifyUserManagementUsersRequest(
                user.DisplayName, organisationSearchResult
                    .Id, _config.ServiceLink));
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

        private int SanitizePageIndex(int? pageIndex)
        {
            return (pageIndex ?? 1) < 0 ? 1 : pageIndex ?? 1;
        }
    }
}