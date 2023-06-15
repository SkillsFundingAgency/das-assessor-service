using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.CharityCommission;
using SFA.DAS.AssessorService.Api.Types.CompaniesHouse;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/organisationsearch")]
    public class OrganisationSearchController : Controller
    {
        private readonly ILogger<OrganisationSearchController> _logger;
        private readonly IOrganisationSearchOrchestrator _organisationSearchOrchestrator;
        private readonly ICompaniesHouseApiClient _companiesHouseApiClient;
        private readonly ICharityCommissionApiClient _charityCommissionApiClient;
        private readonly IValidationService _validationService;

        /// <summary>Initializes a new instance of the <see cref="OrganisationSearchController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="organisationSearchOrchestrator">The organisation search orchestrator.</param>
        /// <param name="companiesHouseApiClient">The companies house API client.</param>
        /// <param name="charityCommissionApiClient">The charity commission API client.</param>
        /// <param name="validationService">The validation service.</param>
        public OrganisationSearchController(ILogger<OrganisationSearchController> logger, IOrganisationSearchOrchestrator organisationSearchOrchestrator, 
            ICompaniesHouseApiClient companiesHouseApiClient, ICharityCommissionApiClient charityCommissionApiClient, IValidationService validationService)
        {
            _logger = logger;
            _organisationSearchOrchestrator = organisationSearchOrchestrator;
            _companiesHouseApiClient = companiesHouseApiClient;
            _charityCommissionApiClient = charityCommissionApiClient;
            _validationService = validationService;
        }

        [HttpGet("organisations")]
        public async Task<PaginatedList<OrganisationSearchResult>> OrganisationSearchPaged(string searchTerm, int pageSize, int pageIndex)
        {
            _logger.LogInformation("Handling Organisation Search Request");

            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                return new PaginatedList<OrganisationSearchResult>(new List<OrganisationSearchResult>(), 0, 1, 1);
            }

            if (_validationService.OrganisationIdIsValid(searchTerm))
            {
                _logger.LogInformation($@"Searching Organisations based on EPAO ID: [{searchTerm}]");
                var orgByEpaoSearchResult = await _organisationSearchOrchestrator.OrganisationSearchByEpao(searchTerm);
                var orgByEpaoSearchResultPaged = orgByEpaoSearchResult.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
                return new PaginatedList<OrganisationSearchResult>(orgByEpaoSearchResultPaged.ToList(), orgByEpaoSearchResult.Count(), pageIndex, pageSize);
            }

            // NOTE: This is required because there are occasions where charity or company number can be interpreted as a ukprn
            var results = new List<OrganisationSearchResult>();
            if (_validationService.UkprnIsValid(searchTerm, out var ukprn))
            {
                _logger.LogInformation($@"Searching Organisations based on UKPRN: [{searchTerm}]");
                var resultFromUkprn = await _organisationSearchOrchestrator.OrganisationSearchByUkprn(ukprn);
                if (resultFromUkprn != null) results.AddRange(resultFromUkprn);
            }

            _logger.LogInformation($@"Searching Organisations based on name or charity number or company number wildcard: [{searchTerm}]");
            var resultFromName = await _organisationSearchOrchestrator.OrganisationSearchByNameOrCharityNumberOrCompanyNumber(searchTerm);
            if (resultFromName != null) results.AddRange(resultFromName);

            var organisationSearchResultList = _organisationSearchOrchestrator.Dedupe(results);
            organisationSearchResultList = organisationSearchResultList.OrderByDescending(x => x.OrganisationIsLive);

            var organisationSearchResultListPaged = organisationSearchResultList.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            return new PaginatedList<OrganisationSearchResult>(organisationSearchResultListPaged.ToList(), organisationSearchResultList.Count(), pageIndex, pageSize);
        }

        [HttpGet("company/{companyNumber}/isActivelyTrading")]
        public async Task<bool> isCompanyActivelyTrading(string companyNumber)
        {

            _logger.LogInformation($"isCompanyActivelyTrading({companyNumber})");

            bool result = false;

            // EPAO Register
            try
            {
                result = await _companiesHouseApiClient.IsCompanyActivelyTrading(companyNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error from Companies House. Message: {ex.Message}");
            }

            return result;
        }

        [HttpGet("company/{companyNumber}")]
        public async Task<Company> GetCompanyDetails(string companyNumber)
        {
            _logger.LogInformation($"GetCompanyDetails({companyNumber})");

            Company company = null;

            try
            {
                company = await _companiesHouseApiClient.GetCompany(companyNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error from Companies House. Message: {ex.Message}");
            }

            return company;
        }

        [HttpGet("charity/{charityNumber}")]
        public async Task<Charity> GetCharityDetails(int charityNumber)
        {
            _logger.LogInformation($"GetCharityDetails({charityNumber})");

            Charity charity = null;

            try
            {
                charity = await _charityCommissionApiClient.GetCharity(charityNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error from Charity Commission. Message: {ex.Message}");
            }

            return charity;
        }
    }
}