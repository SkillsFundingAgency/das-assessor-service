using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Search;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.Application.Infrastructure;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StaffSearchHandler : IRequestHandler<StaffSearchRequest, StaffSearchResult>
    {
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IIlrRepository _ilrRepository;
        private readonly IStaffCertificateRepository _staffCertificateRepository;
        private readonly ILogger<SearchHandler> _logger;
        private readonly IStaffIlrRepository _staffIlrRepository;
        private readonly CacheHelper _cacheHelper;

        public StaffSearchHandler(IAssessmentOrgsApiClient assessmentOrgsApiClient,
            IIlrRepository ilrRepository,
            IStaffCertificateRepository staffCertificateRepository,
            ILogger<SearchHandler> logger,
            IStaffIlrRepository staffIlrRepository, 
            CacheHelper cacheHelper
            )
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _ilrRepository = ilrRepository;
            _staffCertificateRepository = staffCertificateRepository;
            _logger = logger;
            _staffIlrRepository = staffIlrRepository;
            _cacheHelper = cacheHelper;
        }

        public async Task<StaffSearchResult> Handle(StaffSearchRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.SearchQuery))
                return new StaffSearchResult
                {
                    EndpointAssessorOrganisationId = String.Empty,
                    StaffSearchItems =
                        new PaginatedList<StaffSearchItems>(new List<StaffSearchItems>(), 0, request.Page, 10)
                };

            var pageSize = 10;
            var searchResult = await Search(request);
            var totalRecordCount = searchResult.TotalCount;

            var displayEpao = false;
            if (searchResult.TotalCount == 0)
            {
                totalRecordCount = await _staffIlrRepository.CountLearnersByName(request.SearchQuery);
                searchResult.PageOfResults =
                    await _staffIlrRepository.SearchForLearnerByName(request.SearchQuery, request.Page, pageSize);
            }
            else
            {
                displayEpao = searchResult.DisplayEpao;
            }

            _logger.LogInformation(searchResult.PageOfResults.Any()
                ? LoggingConstants.SearchSuccess
                : LoggingConstants.SearchFailure);

            var searchResults = Mapper.Map<List<StaffSearchItems>>(searchResult.PageOfResults);

            searchResults = MatchUpExistingCompletedStandards(searchResults);
            searchResults = await PopulateStandards(searchResults, _assessmentOrgsApiClient, _logger);

            return new StaffSearchResult
            {
                DisplayEpao = displayEpao,
                EndpointAssessorOrganisationId = displayEpao && searchResults.Count > 0
                    ? searchResults.First().EndpointAssessorOrganisationId
                    : string.Empty,
                StaffSearchItems =
                    new PaginatedList<StaffSearchItems>(searchResults, totalRecordCount, request.Page, pageSize)
            };
        }

        private async Task<StaffReposSearchResult> Search(StaffSearchRequest request)
        {
            // Naive decision on what is being searched.

            var regex = new Regex(@"\b(?i)(epa)[0-9]{4}\b");
            if (regex.IsMatch(request.SearchQuery))
            {
                var sr = await _staffIlrRepository.SearchForLearnerByEpaOrgId(request);
                sr.DisplayEpao = true;
                return sr;
            }

            if (request.SearchQuery.Length == 10 && long.TryParse(request.SearchQuery, out var uln))
            {
                if (await _staffCertificateRepository.IsPrivateCertificateForUln(Convert.ToInt64(request.SearchQuery)))
                {
                    var certificates = (await _staffCertificateRepository.SearchForLearnerByUln(uln));
                    var sr = new StaffReposSearchResult
                    {
                        TotalCount = certificates.Count(),
                        PageOfResults = certificates
                    };
                    return sr;
                }
                else
                {
                    // Search string is a long of 10 length so must be a uln.
                    var sr = new StaffReposSearchResult
                    {
                        TotalCount = (await _ilrRepository.SearchForLearnerByUln(uln)).Count(),
                        PageOfResults = await _staffIlrRepository.SearchForLearnerByUln(request)
                    };
                    return sr;    
                }
               
            }

            if (request.SearchQuery.Length == 8 && long.TryParse(request.SearchQuery, out var certRef))
            {
                if (await _staffCertificateRepository.IsPrivateCertificateForCertificateReference(request.SearchQuery))
                {
                    var learnerCertificateRequest =
                        await _staffCertificateRepository.SearchForLearnerByCertificateReference(request.SearchQuery);
                    var sr = new StaffReposSearchResult
                    {
                        DisplayEpao = true,
                        TotalCount = learnerCertificateRequest.Count(),
                        PageOfResults = learnerCertificateRequest
                    };
                    return sr;
                }
                else
                {
                    var learnerCertificateRequest =
                        await _staffIlrRepository.SearchForLearnerByCertificateReference(request.SearchQuery);
                    var sr = new StaffReposSearchResult
                    {
                        DisplayEpao = true,
                        TotalCount = learnerCertificateRequest.Count(),
                        PageOfResults = learnerCertificateRequest
                    };
                    return sr;
                }
            }

            return new StaffReposSearchResult() {PageOfResults = new List<Ilr>(), TotalCount = 0};
        }


        private List<StaffSearchItems> MatchUpExistingCompletedStandards(List<StaffSearchItems> searchResults)
        {
            _logger.LogInformation("MatchUpExistingCompletedStandards Before Get Certificates for uln from db");
            var completedCertificates = _staffCertificateRepository
                .GetCertificatesFor(searchResults.Select(r => r.Uln).ToArray()).Result;
            _logger.LogInformation("MatchUpExistingCompletedStandards After Get Certificates for uln from db");

            foreach (var searchResult in searchResults)
            {
                var certificate = completedCertificates.SingleOrDefault(s =>
                    s.StandardCode == searchResult.StandardCode && s.Uln == searchResult.Uln);
                if (certificate == null) continue;

                searchResult.CertificateReference = certificate.CertificateReference;
                searchResult.CertificateStatus = certificate.Status;
                searchResult.GivenNames = certificate.GivenNames;
                searchResult.FamilyName = certificate.FamilyName;
                searchResult.EndpointAssessorOrganisationId = certificate.EndPointAssessorOrganisationId;

                if (searchResult.LastUpdatedAt == null)
                {
                    searchResult.LastUpdatedAt = certificate.LastUpdatedAt?.UtcToTimeZoneTime();
                }
            }

            return searchResults;
        }

        private async Task<List<StaffSearchItems>> PopulateStandards(List<StaffSearchItems> searchResults,
            IAssessmentOrgsApiClient assessmentOrgsApiClient, ILogger<SearchHandler> logger)
        {
            var results = await _cacheHelper.RetrieveFromCache<IEnumerable<StandardSummary>>("AllStandardSummaries");
            if (results == null)
            {
                var standards = assessmentOrgsApiClient.GetAllStandardSummaries().Result;
                await _cacheHelper.SaveToCache("AllStandardSummaries", standards, 1);

                results = standards;
            }
                     
            foreach (var searchResult in searchResults)
            {
                if (searchResult.StandardCode != 0)
                {
                    var standard =
                        results.SingleOrDefault(s => s.Id == searchResult.StandardCode.ToString());

                    searchResult.Standard = standard.Title;
                }0
            }

            return searchResults;
        }
    }
}