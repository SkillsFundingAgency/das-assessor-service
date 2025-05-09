﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.CharityCommission;
using SFA.DAS.AssessorService.Api.Types.CompaniesHouse;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.CharityCommission;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.CompaniesHouse;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.ReferenceData;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/organisationsearch")]
    public class OrganisationSearchController : Controller
    {
        private readonly ILogger<OrganisationSearchController> _logger;
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly IReferenceDataApiClient _referenceDataApiClient;
        private readonly ICompaniesHouseApiClient _companiesHouseApiClient;
        private readonly ICharityCommissionApiClient _charityCommissionApiClient;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public OrganisationSearchController(ILogger<OrganisationSearchController> logger, IMediator mediator, IRoatpApiClient roatpApiClient, IReferenceDataApiClient referenceDataApiClient, ICompaniesHouseApiClient companiesHouseApiClient, ICharityCommissionApiClient charityCommissionApiClient, IMapper mapper)
        {
            _logger = logger;
            _roatpApiClient = roatpApiClient;
            _referenceDataApiClient = referenceDataApiClient;
            _companiesHouseApiClient = companiesHouseApiClient;
            _charityCommissionApiClient = charityCommissionApiClient;
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("organisations")]
        public async Task<PaginatedList<OrganisationSearchResult>> OrganisationSearchPaged(string searchTerm, int pageSize, int pageIndex)
        {
            _logger.LogInformation("Handling Organisation Search Request");
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                return new PaginatedList<OrganisationSearchResult>(new List<OrganisationSearchResult>(), 0, 1, 1);
            }

            var results = new List<OrganisationSearchResult>();
            if (IsValidEpaOrganisationId(searchTerm))
            {
                _logger.LogInformation($@"Searching Organisations based on EPAO ID: [{searchTerm}]");
                var resultFromEpaOrganisationId = await OrganisationSearchByEpao(searchTerm);
                if (resultFromEpaOrganisationId != null) results.AddRange(resultFromEpaOrganisationId);
            }
            else
            {
                // NOTE: This is required because there are occasions where charity or company number can be interpreted as a ukprn
                if (IsValidUkprn(searchTerm, out var ukprn))
                {
                    _logger.LogInformation($@"Searching Organisations based on UKPRN: [{searchTerm}]");
                    var resultFromUkprn = await OrganisationSearchByUkprn(ukprn);
                    if (resultFromUkprn != null) results.AddRange(resultFromUkprn);
                }

                _logger.LogInformation($@"Searching Organisations based on name or charity number or company number wildcard: [{searchTerm}]");
                var resultFromName = await OrganisationSearchByNameOrCharityNumberOrCompanyNumber(searchTerm);
                if (resultFromName != null) results.AddRange(resultFromName);
            }

            var organisationSearchResultList = Dedupe(results);
            organisationSearchResultList = organisationSearchResultList
                .Where(p => p.OrganisationIsLive == true)
                .OrderByDescending(x => x.OrganisationIsLive);

            var organisationSearchResultListPaged = organisationSearchResultList.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            return new PaginatedList<OrganisationSearchResult>(organisationSearchResultListPaged.ToList(), organisationSearchResultList.Count(), pageIndex, pageSize);
        }


        private bool IsValidEpaOrganisationId(string organisationIdToCheck)
        {
            var regex = new Regex(@"[eE][pP][aA][0-9]{4,9}$");
            return regex.Match(organisationIdToCheck).Success;
        }

        private bool IsValidUkprn(string stringToCheck, out int ukprn)
        {
            if (!int.TryParse(stringToCheck, out ukprn))
            {
                return false;
            }

            return ukprn >= 10000000 && ukprn <= 99999999;
        }

        private async Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByUkprn(int ukprn)
        {
            IEnumerable<OrganisationSearchResult> epaoResults = await GetEpaoRegisterResults(ukprn.ToString());
            //IEnumerable<OrganisationSearchResult> roatpResults = await GetAtpRegisterResults(null, null, ukprn);
            //IEnumerable<OrganisationSearchResult> providerResults;
            //IEnumerable<OrganisationSearchResult> referenceResults;

            //var providerRegisterNames = new List<string>();
            //if (epaoResults?.Count() == 1)
            //{
            //    providerRegisterNames.Add(epaoResults.First().TradingName);
            //    providerRegisterNames.Add(epaoResults.First().LegalName);
            //}
            //if (roatpResults?.Count() == 1)
            //{
            //    providerRegisterNames.Add(roatpResults.First().ProviderName);
            //}
            //providerResults = await GetProviderRegisterResults(null, providerRegisterNames, ukprn);

            //// If you try to search Reference Data API by UKPRN it interprets this as Company Number so must use actual name instead
            //var referenceDataApiNames = new List<string>(providerRegisterNames);
            //if (providerResults?.Count() == 1)
            //{
            //    referenceDataApiNames.Add(providerResults.First().ProviderName);
            //}
            //referenceResults = await GetReferenceDataResults(null, referenceDataApiNames, ukprn);

            var results = new List<OrganisationSearchResult>();
            if (epaoResults != null) results.AddRange(epaoResults);
            //if (roatpResults != null) results.AddRange(roatpResults);
            //if (providerResults != null) results.AddRange(providerResults);
            //if (referenceResults != null) results.AddRange(referenceResults);

            return Dedupe(results);
        }

        private async Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByEpao(string epaoId)
        {
            IEnumerable<OrganisationSearchResult> epaoResults = await GetEpaoRegisterResults(epaoId);
            //IEnumerable<OrganisationSearchResult> roatpResults;
            //IEnumerable<OrganisationSearchResult> providerResults;
            //IEnumerable<OrganisationSearchResult> referenceResults;
            //int? ukprn = null;

            //var atpRegisterNames = new List<string>();
            //if (epaoResults?.Count() == 1)
            //{
            //    atpRegisterNames.Add(epaoResults.First().TradingName);
            //    atpRegisterNames.Add(epaoResults.First().LegalName);
            //    ukprn = epaoResults.First().Ukprn;
            //}
            //roatpResults = await GetAtpRegisterResults(null, atpRegisterNames, ukprn);

            //var providerRegisterNames = new List<string>(atpRegisterNames);
            //if (roatpResults?.Count() == 1)
            //{
            //    providerRegisterNames.Add(roatpResults.First().ProviderName);
            //}
            //providerResults = await GetProviderRegisterResults(null, providerRegisterNames, ukprn);

            //// If you try to search Reference Data API by EPAO ID it interprets this as Company Name so must use actual name instead
            //var referenceDataApiNames = new List<string>(providerRegisterNames);
            //if (providerResults?.Count() == 1)
            //{
            //    referenceDataApiNames.Add(providerResults.First().ProviderName);
            //}
            //referenceResults = await GetReferenceDataResults(null, referenceDataApiNames, ukprn);

            var results = new List<OrganisationSearchResult>();
            if (epaoResults != null) results.AddRange(epaoResults);
            //if (roatpResults != null) results.AddRange(roatpResults);
            //if (providerResults != null) results.AddRange(providerResults);
            //if (referenceResults != null) results.AddRange(referenceResults);

            return Dedupe(results);
        }

        private async Task<IEnumerable<OrganisationSearchResult>> OrganisationSearchByNameOrCharityNumberOrCompanyNumber(string name)
        {
            IEnumerable<OrganisationSearchResult> epaoResults = await GetEpaoRegisterResults(name);
            //IEnumerable<OrganisationSearchResult> roatpResults;
            //IEnumerable<OrganisationSearchResult> providerResults;
            //IEnumerable<OrganisationSearchResult> referenceResults;
            //int? ukprn = null;

            //var atpRegisterNames = new List<string>();
            //if (epaoResults?.Count() == 1)
            //{
            //    atpRegisterNames.Add(epaoResults.First().TradingName);
            //    atpRegisterNames.Add(epaoResults.First().LegalName);
            //    ukprn = epaoResults.First().Ukprn;
            //}
            //roatpResults = await GetAtpRegisterResults(name, atpRegisterNames, ukprn);

            //var providerRegisterNames = new List<string>(atpRegisterNames);
            //if (roatpResults?.Count() == 1)
            //{
            //    providerRegisterNames.Add(roatpResults.First().ProviderName);
            //}
            //providerResults = await GetProviderRegisterResults(name, providerRegisterNames, ukprn);

            //var referenceDataApiNames = new List<string>(providerRegisterNames);
            //if (providerResults?.Count() == 1)
            //{
            //    referenceDataApiNames.Add(providerResults.First().ProviderName);
            //}
            //referenceResults = await GetReferenceDataResults(name, referenceDataApiNames, ukprn);

            var results = new List<OrganisationSearchResult>();
            if (epaoResults != null) results.AddRange(epaoResults);
            //if (roatpResults != null) results.AddRange(roatpResults);
            //if (providerResults != null) results.AddRange(providerResults);
            //if (referenceResults != null) results.AddRange(referenceResults);

            return Dedupe(results);
        }

        private async Task<IEnumerable<OrganisationSearchResult>> GetEpaoRegisterResults(string searchTerm)
        {
            var results = new List<OrganisationSearchResult>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                try
                {
                    _logger.LogInformation($@"Search Assessment Organisations for [{searchTerm}]");
                    var apiResponse = await _mediator.Send(new SearchAssessmentOrganisationsRequest { SearchTerm = searchTerm });
                    var organisationSearchResults =
                        _mapper.Map<IEnumerable<AssessmentOrganisationSummary>, IEnumerable<OrganisationSearchResult>>(apiResponse);
                    if (organisationSearchResults != null) results.AddRange(organisationSearchResults);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error from EPAO Register. Search Term: {searchTerm} , Message: {ex.Message}");
                }
            }

            return results.ToList();
        }

        private async Task<IEnumerable<OrganisationSearchResult>> GetAtpRegisterResults(string name, IEnumerable<string> exactNames, int? ukprn)
        {
            var results = new List<OrganisationSearchResult>();

            if (ukprn.HasValue)
            {
                try
                {
                    var response = await _roatpApiClient.SearchOrganisationByUkprn(ukprn.Value);
                    if (response != null) results.AddRange(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error from ATP Register. UKPRN: {ukprn} , Message: {ex.Message}");
                }
            }

            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    var response = await _roatpApiClient.SearchOrganisationByName(name, false);
                    if (response != null) results.AddRange(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error from ATP Register. {name} , Message: {ex.Message}");
                }
            }

            if (exactNames != null)
            {
                foreach (var exactName in exactNames)
                {
                    try
                    {
                        var response = await _roatpApiClient.SearchOrganisationByName(exactName, true);
                        if (response != null) results.AddRange(response);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error from ATP Register. Exact Name: {exactName} , Message: {ex.Message}");
                    }
                }
            }

            return results.GroupBy(r => r.Ukprn).Select(group => group.First()).ToList();
        }

        private async Task<IEnumerable<OrganisationSearchResult>> GetProviderRegisterResults(string name, IEnumerable<string> exactNames, int? ukprn)
        {
            var results = new List<OrganisationSearchResult>();

            if (ukprn.HasValue)
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
            }

            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    var response = await _roatpApiClient.SearchOrganisationByName(name, false);
                    if (response != null) results.AddRange(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error from Provider Register. {name} , Message: {ex.Message}");
                }
            }

            if (exactNames != null)
            {
                foreach (var exactName in exactNames)
                {
                    try
                    {
                        var response = await _roatpApiClient.SearchOrganisationByName(exactName, true);
                        if (response != null) results.AddRange(response);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error from Provider Register. Exact Name: {exactName} , Message: {ex.Message}");
                    }
                }
            }

            return results.GroupBy(r => r.Ukprn).Select(group => group.First()).ToList();
        }

        private async Task<IEnumerable<OrganisationSearchResult>> GetReferenceDataResults(string name, IEnumerable<string> exactNames, int? ukprn)
        {
            var results = new List<OrganisationSearchResult>();

            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    var response = await _referenceDataApiClient.SearchOrgansiation(name, false);
                    if (response != null) results.AddRange(response);

                    if (int.TryParse(name, out int companyOrCharityNumber))
                    {
                        // NOTE: API requires leading zeroes in order to search company number or charity number
                        var response2 = await _referenceDataApiClient.SearchOrgansiation(companyOrCharityNumber.ToString("D8"), false);
                        if (response2 != null) results.AddRange(response2);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error from Reference Data Api. Name: {name} , Message: {ex.Message}");
                }
            }

            if (exactNames != null)
            {
                foreach (var exactName in exactNames)
                {
                    try
                    {
                        var response = await _referenceDataApiClient.SearchOrgansiation(exactName, true);
                        if (response != null)
                        {
                            if (ukprn.HasValue)
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
            }

            return results;
        }

        private IEnumerable<OrganisationSearchResult> Dedupe(IEnumerable<OrganisationSearchResult> organisations)
        {
            var nameMerge = organisations.GroupBy(org => org.Name.ToUpperInvariant())
                .Select(group =>
                    new OrganisationSearchResult
                    {
                        Id = group.Select(g => g.Id).FirstOrDefault(Id => !string.IsNullOrWhiteSpace(Id)),
                        Ukprn = group.Select(g => g.Ukprn).FirstOrDefault(Ukprn => Ukprn.HasValue),
                        LegalName = group.Select(g => g.LegalName).FirstOrDefault(LegalName => !string.IsNullOrWhiteSpace(LegalName)),
                        TradingName = group.Select(g => g.TradingName).FirstOrDefault(TradingName => !string.IsNullOrWhiteSpace(TradingName)),
                        ProviderName = group.Select(g => g.ProviderName).FirstOrDefault(ProviderName => !string.IsNullOrWhiteSpace(ProviderName)),
                        Address = group.Select(g => g.Address).FirstOrDefault(Address => Address != null),
                        OrganisationType = group.Select(g => g.OrganisationType).FirstOrDefault(OrganisationType => !string.IsNullOrWhiteSpace(OrganisationType)),
                        OrganisationReferenceType = group.Select(g => g.OrganisationReferenceType).FirstOrDefault(OrganisationReferenceType => !string.IsNullOrWhiteSpace(OrganisationReferenceType)),
                        OrganisationReferenceId = string.Join(",", group.Select(g => g.Id).Where(Id => !string.IsNullOrWhiteSpace(Id)).Distinct()),
                        RoEPAOApproved = group.Select(g => g.RoEPAOApproved).FirstOrDefault(RoEPAOApproved => RoEPAOApproved != false),
                        RoATPApproved = group.Select(g => g.RoATPApproved).FirstOrDefault(RoATPApproved => RoATPApproved != false),
                        CompanyNumber = group.Select(g => g.CompanyNumber).FirstOrDefault(CompanyNumber => !string.IsNullOrWhiteSpace(CompanyNumber)),
                        CharityNumber = group.Select(g => g.CharityNumber).FirstOrDefault(CharityNumber => !string.IsNullOrWhiteSpace(CharityNumber)),
                        EasApiOrganisationType = group.Select(g => g.EasApiOrganisationType).FirstOrDefault(EasApiOrganisationType => !string.IsNullOrWhiteSpace(EasApiOrganisationType)),
                        FinancialDueDate = group.Select(g => g.FinancialDueDate).FirstOrDefault(FinancialDueDate => FinancialDueDate != null),
                        FinancialExempt = group.Select(g => g.FinancialExempt).FirstOrDefault(FinancialExempt => FinancialExempt != null),
                        OrganisationIsLive = group.Select(g => g.OrganisationIsLive).FirstOrDefault(OrganisationIsAlive => OrganisationIsAlive)
                    }
                );

            var ukprnMerge = nameMerge.GroupBy(org => new { filter = org.Ukprn.HasValue ? org.Ukprn.ToString() : org.Name.ToUpperInvariant() })
                .Select(group =>
                    new OrganisationSearchResult
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
                        OrganisationIsLive = group.Select(g => g.OrganisationIsLive).FirstOrDefault(OrganisationIsAlive => OrganisationIsAlive)
                    }
                );

            var companyNumberMerge = ukprnMerge.GroupBy(org => new { filter = org.CompanyNumber != null ? org.CompanyNumber.PadLeft(8, '0') : org.Name.ToUpperInvariant() })
                .Select(group =>
                    new OrganisationSearchResult
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
                        OrganisationIsLive = group.Select(g => g.OrganisationIsLive).FirstOrDefault(OrganisationIsAlive => OrganisationIsAlive)
                    }
                );

            var charityNumberMerge = companyNumberMerge.GroupBy(org => new { filter = org.CharityNumber != null ? org.CharityNumber.PadLeft(8, '0') : org.Name.ToUpperInvariant() })
                .Select(group =>
                    new OrganisationSearchResult
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
                        OrganisationIsLive = group.Select(g => g.OrganisationIsLive).FirstOrDefault(OrganisationIsLive => OrganisationIsLive)
                    }
                );

            return charityNumberMerge.OrderByDescending(org => org.Ukprn).ToList();
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