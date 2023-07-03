using AutoMapper;
using CharityCommissionService;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Infrastructure;
using SFA.DAS.AssessorService.Application.Infrastructure;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Orchestrators
{
    /// <summary>
    /// Searches for the organisation
    /// </summary>
    public class OrganisationSearchOrchestrator : IOrganisationSearchOrchestrator
    {
        private readonly ILogger<OrganisationSearchOrchestrator> _logger;
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly IReferenceDataApiClient _referenceDataApiClient;
        private readonly IMediator _mediator;

        /// <summary>Initializes a new instance of the <see cref="OrganisationSearchOrchestrator" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="roatpApClient">The roatp ap client.</param>
        /// <param name="referenceDataApClient">The reference data ap client.</param>
        /// <param name="mediator">The mediator.</param>
        public OrganisationSearchOrchestrator(ILogger<OrganisationSearchOrchestrator> logger, IRoatpApiClient roatpApClient, IReferenceDataApiClient referenceDataApClient, IMediator mediator)
        {
            _logger = logger;
            _roatpApiClient = roatpApClient;
            _referenceDataApiClient = referenceDataApClient;
            _mediator = mediator;
        }

        /// <summary>Organisations search by ukprn.</summary>
        /// <param name="ukprn">The ukprn.</param>
        public async Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByUkprn(int ukprn)
        {
            IEnumerable<OrganisationSearchResult> epaoResults = await GetEpaoRegisterResults(ukprn.ToString());
            IEnumerable<OrganisationSearchResult> roatpResults = await GetRegisterResults(null, null, ukprn);
            IEnumerable<OrganisationSearchResult> providerResults;
            IEnumerable<OrganisationSearchResult> referenceResults;

            var providerRegisterNames = new List<string>();
            if (epaoResults?.Count() == 1)
            {
                providerRegisterNames.Add(epaoResults.First().TradingName);
                providerRegisterNames.Add(epaoResults.First().LegalName);
            }
            if (roatpResults?.Count() == 1)
            {
                providerRegisterNames.Add(roatpResults.First().ProviderName);
            }
            providerResults = await GetRegisterResults(null, providerRegisterNames, ukprn);

            // if Reference Data API is searched by UKPRN it interprets this as Company Number so must use actual name instead
            var referenceDataApiNames = new List<string>(providerRegisterNames);
            if (providerResults?.Count() == 1)
            {
                referenceDataApiNames.Add(providerResults.First().ProviderName);
            }
            referenceResults = await GetReferenceDataResults(null, referenceDataApiNames, ukprn);

            return Dedupe(epaoResults.Concat(roatpResults).Concat(providerResults).Concat(referenceResults));
        }

        /// <summary>Organisations search by Epao.</summary>
        /// <param name="epaoId"></param>
        public async Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByEpao(string epaoId)
        {
            IEnumerable<OrganisationSearchResult> epaoResults = await GetEpaoRegisterResults(epaoId);
            IEnumerable<OrganisationSearchResult> roatpResults;
            IEnumerable<OrganisationSearchResult> providerResults;
            IEnumerable<OrganisationSearchResult> referenceResults;
            int? ukprn = null;

            var atpRegisterNames = new List<string>();
            if (epaoResults?.Count() == 1)
            {
                atpRegisterNames.Add(epaoResults.First().TradingName);
                atpRegisterNames.Add(epaoResults.First().LegalName);
                ukprn = epaoResults.First().Ukprn;
            }
            roatpResults = await GetRegisterResults(null, atpRegisterNames, ukprn);

            var providerRegisterNames = new List<string>(atpRegisterNames);
            if (roatpResults?.Count() == 1)
            {
                providerRegisterNames.Add(roatpResults.First().ProviderName);
            }
            providerResults = await GetRegisterResults(null, providerRegisterNames, ukprn);

            // if Reference Data API is searched by EPAO ID it interprets this as Company Number so must use actual name instead
            var referenceDataApiNames = new List<string>(providerRegisterNames);
            if (providerResults?.Count() == 1)
            {
                referenceDataApiNames.Add(providerResults.First().ProviderName);
            }
            referenceResults = await GetReferenceDataResults(null, referenceDataApiNames, ukprn);

            return Dedupe(epaoResults.Concat(roatpResults).Concat(providerResults).Concat(referenceResults));
        }

        /// <summary>Organisations search by name or charity number or company number.</summary>
        /// <param name="searchTerm">The search term.</param>
        public async Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByNameOrCharityNumberOrCompanyNumber(string searchTerm)
        {
            IEnumerable<OrganisationSearchResult> epaoResults = await GetEpaoRegisterResults(searchTerm);
            IEnumerable<OrganisationSearchResult> roatpResults;
            IEnumerable<OrganisationSearchResult> providerResults;
            IEnumerable<OrganisationSearchResult> referenceResults;
            int? ukprn = null;

            var atpRegisterNames = new List<string>();
            if (epaoResults?.Count() == 1)
            {
                atpRegisterNames.Add(epaoResults.First().TradingName);
                atpRegisterNames.Add(epaoResults.First().LegalName);
                ukprn = epaoResults.First().Ukprn;
            }
            roatpResults = await GetRegisterResults(searchTerm, atpRegisterNames, ukprn);

            var providerRegisterNames = new List<string>(atpRegisterNames);
            if (roatpResults?.Count() == 1)
            {
                providerRegisterNames.Add(roatpResults.First().ProviderName);
            }
            providerResults = await GetRegisterResults(searchTerm, providerRegisterNames, ukprn);

            var referenceDataApiNames = new List<string>(providerRegisterNames);
            if (providerResults?.Count() == 1)
            {
                referenceDataApiNames.Add(providerResults.First().ProviderName);
            }
            referenceResults = await GetReferenceDataResults(searchTerm, referenceDataApiNames, ukprn);

            // for any results found search the register again using the company number 
            // and charity number to pickup cases where the company name or charity name
            // has changed since the previous registration, this will then return the
            // previous registration which has the same company number or charity number
            if (referenceResults.Any())
            {
                var numbers = referenceResults
                    .SelectMany(f => new List<string>() { f.CharityNumber, f.CompanyNumber })
                    .Where(number => !string.IsNullOrEmpty(number))
                    .Distinct();

                var additionalEpaoResults = await GetAdditionalEpaoRegisterResults(numbers);

                epaoResults = epaoResults
                    .Concat(additionalEpaoResults);
            }

            return Dedupe(epaoResults.Concat(roatpResults).Concat(providerResults).Concat(referenceResults));
        }

        /// <summary>Dedupes the specified organisations.</summary>
        /// <param name="organisations">The organisations.</param>
        public IEnumerable<OrganisationSearchResult> Dedupe(IEnumerable<OrganisationSearchResult> organisations)
        {
            var nameMerge = organisations.GroupBy(org => new { filter = org.Name.ToUpperInvariant() })
                .Select(group => DedupeTransform(group));

            var ukprnMerge = nameMerge.GroupBy(org => new { filter = org.Ukprn.HasValue ? org.Ukprn.ToString() : org.Name.ToUpperInvariant() })
                .Select(group => DedupeTransform(group));

            var companyNumberMerge = ukprnMerge.GroupBy(org => new { filter = org.CompanyNumber != null ? org.CompanyNumber.PadLeft(8, '0') : org.Name.ToUpperInvariant() })
                .Select(group => DedupeTransform(group));

            var charityNumberMerge = companyNumberMerge.GroupBy(org => new { filter = org.CharityNumber != null ? org.CharityNumber.PadLeft(8, '0') : org.Name.ToUpperInvariant() })
                .Select(group => DedupeTransform(group));

            return charityNumberMerge.OrderByDescending(org => org.Ukprn).ToList();
        }

        private OrganisationSearchResult DedupeTransform(IGrouping<dynamic, OrganisationSearchResult> group)
        {
            return new OrganisationSearchResult
            {
                Id = group.Select(g => g.Id).FirstOrDefault(Id => !string.IsNullOrWhiteSpace(Id)),
                Ukprn = group.Select(g => g.Ukprn).FirstOrDefault(Ukprn => Ukprn.HasValue),
                LegalName = group.Select(g => g.LegalName).FirstOrDefault(LegalName => !string.IsNullOrWhiteSpace(LegalName)),
                TradingName = group.Select(g => g.TradingName).FirstOrDefault(TradingName => !string.IsNullOrWhiteSpace(TradingName)),
                ProviderName = group.Select(g => g.ProviderName).FirstOrDefault(ProviderName => !string.IsNullOrWhiteSpace(ProviderName)),
                Address = group.Select(g => g.Address).FirstOrDefault(Address => Address != null),
                OrganisationType = group.Select(g => g.OrganisationType).FirstOrDefault(OrganisationType => !string.IsNullOrWhiteSpace(OrganisationType)),
                OrganisationReferenceType = group.Select(g => g.OrganisationReferenceType).FirstOrDefault(OrganisationReferenceType => !string.IsNullOrWhiteSpace(OrganisationReferenceType)),
                OrganisationReferenceId = string.Join(",", group.Select(g => g.OrganisationReferenceId).Where(Id => !string.IsNullOrWhiteSpace(Id)).Distinct()),
                RoEPAOApproved = group.Select(g => g.RoEPAOApproved).FirstOrDefault(RoEPAOApproved => RoEPAOApproved != false),
                RoATPApproved = group.Select(g => g.RoATPApproved).FirstOrDefault(RoATPApproved => RoATPApproved != false),
                CompanyNumber = group.Select(g => g.CompanyNumber).FirstOrDefault(CompanyNumber => !string.IsNullOrWhiteSpace(CompanyNumber)),
                CharityNumber = group.Select(g => g.CharityNumber).FirstOrDefault(CharityNumber => !string.IsNullOrWhiteSpace(CharityNumber)),
                EasApiOrganisationType = group.Select(g => g.EasApiOrganisationType).FirstOrDefault(EasApiOrganisationType => !string.IsNullOrWhiteSpace(EasApiOrganisationType)),
                FinancialDueDate = group.Select(g => g.FinancialDueDate).FirstOrDefault(FinancialDueDate => FinancialDueDate != null),
                FinancialExempt = group.Select(g => g.FinancialExempt).FirstOrDefault(FinancialExempt => FinancialExempt != null),
                OrganisationIsLive = group.Select(g => g.OrganisationIsLive).FirstOrDefault()
            };
        }

        private async Task<IEnumerable<OrganisationSearchResult>> GetEpaoRegisterResults(string searchTerm)
        {
            var results = new List<OrganisationSearchResult>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                try
                {
                    _logger.LogDebug($@"Search Assessment Organisations for [{searchTerm}]");

                    var response = await _mediator.Send(new SearchAssessmentOrganisationsRequest { SearchTerm = searchTerm });

                    var organisationSearchResults =
                        Mapper.Map<IEnumerable<AssessmentOrganisationSummary>, IEnumerable<OrganisationSearchResult>>(response);

                    if (organisationSearchResults != null)
                        results.AddRange(organisationSearchResults);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error from EPAO Register. Search Term: {searchTerm}, Message: {ex.Message}");
                }
            }

            return results;
        }

        private async Task<IEnumerable<OrganisationSearchResult>> GetAdditionalEpaoRegisterResults(IEnumerable<string> numbers)
        {
            var results = new List<OrganisationSearchResult>();

            if (numbers.Any())
            {
                try
                {
                    var response = await _mediator.Send(new GetAssessmentOrganisationsByCharityNumbersOrCompanyNumbersRequest { Numbers = numbers });

                    var organisationSearchResults =
                        Mapper.Map<IEnumerable<AssessmentOrganisationSummary>, IEnumerable<OrganisationSearchResult>>(response);

                    if (organisationSearchResults != null)
                        results.AddRange(organisationSearchResults);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error from EPAO Register. Numbers: {numbers}, Message: {ex.Message}");
                }
            }

            return results;
        }

        private async Task<IEnumerable<OrganisationSearchResult>> GetRegisterResults(string name, IEnumerable<string> exactNames, int? ukprn)
        {
            var results = new List<OrganisationSearchResult>();

            if (ukprn.HasValue)
            {
                results = await SearchRoatpOrganisationByUkprn(ukprn.Value, results);
            }

            if (!string.IsNullOrEmpty(name))
            {
                results = await SearchRoatpOrganisationByName(name, false, results);
            }

            if (exactNames != null)
            {
                foreach (var exactName in exactNames)
                {
                    results = await SearchRoatpOrganisationByName(exactName, true,results);                    
                }
            }

            return results.GroupBy(r => r.Ukprn).Select(group => group.First()).ToList();
        }     

        private async Task<IEnumerable<OrganisationSearchResult>> GetReferenceDataResults(string name, IEnumerable<string> exactNames, int? ukprn)
        {
            var results = new List<OrganisationSearchResult>();

            if (!string.IsNullOrEmpty(name))
            {
                results = await SearchReferenceDataByNameOrCharityNumber(name, false, results);
            }

            if (exactNames != null)
            {
                results = await SearchReferenceDataByExactNames(exactNames, ukprn, true, results);                
            }

            return results;
        }

        private async Task<List<OrganisationSearchResult>> SearchRoatpOrganisationByName(string searchTerm, bool exactMatch, List<OrganisationSearchResult> results)
        {
            try
            {
                var response = await _roatpApiClient.SearchOrganisationByName(searchTerm, exactMatch);
                if (response != null) results.AddRange(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error from Provider Register. {searchTerm} , Message: {ex.Message}");
            }
            return results;
        }

        private async Task<List<OrganisationSearchResult>> SearchRoatpOrganisationByUkprn(int? ukprn, List<OrganisationSearchResult> results)
        {
            try
            {
                var ukprnResponse = await _roatpApiClient.GetOrganisationByUkprn(ukprn.Value);
                if (ukprnResponse != null) results.Add(ukprnResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error from Provider Register. UKPRN: {ukprn.Value} , Message: {ex.Message}");
            }
            return results;
        }

        private async Task<List<OrganisationSearchResult>> SearchReferenceDataByNameOrCharityNumber(string name, bool exactMatch, List<OrganisationSearchResult> results)
        {
            try
            {
                var response = await _referenceDataApiClient.SearchOrgansiation(name, exactMatch);
                if (response != null) results.AddRange(response);

                if (int.TryParse(name, out int companyOrCharityNumber))
                {
                    // NOTE: API requires leading zeroes in order to search company number or charity number
                    var response2 = await _referenceDataApiClient.SearchOrgansiation(companyOrCharityNumber.ToString("D8"), exactMatch);
                    if (response2 != null) results.AddRange(response2);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error from Reference Data Api. Name: {name} , Message: {ex.Message}");
            }

            return results;
        }

        private async Task<List<OrganisationSearchResult>> SearchReferenceDataByExactNames(IEnumerable<string> exactNames, int? ukprn, bool exactMatch, List<OrganisationSearchResult> results)
        {
            foreach (var exactName in exactNames)
            {
                try
                {
                    var response = await _referenceDataApiClient.SearchOrgansiation(exactName, exactMatch);
                    if (ukprn.HasValue)
                    {
                        if (response != null)
                        {
                            // The results from this API don't currently return UKPRN
                            foreach (var r in response)
                            {
                                r.Ukprn = ukprn;
                            }
                        }

                        results.AddRange(response);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error from Provider Register. Exact Name: {exactName} , Message: {ex.Message}");
                }                
            }

            return results;
        }
    }
}
