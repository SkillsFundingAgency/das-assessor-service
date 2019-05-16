using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Search;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StaffSearchHandler : IRequestHandler<StaffSearchRequest, StaffSearchResult>
    {
        private readonly IStaffCertificateRepository _staffCertificateRepository;
        private readonly ILogger<SearchHandler> _logger;
        private readonly IStaffIlrRepository _staffIlrRepository;
        private readonly IStandardService _standardService;

        public StaffSearchHandler(IStaffCertificateRepository staffCertificateRepository,
            ILogger<SearchHandler> logger,
            IStaffIlrRepository staffIlrRepository, 
            IStandardService staffService)
        {
            _staffCertificateRepository = staffCertificateRepository;
            _logger = logger;
            _staffIlrRepository = staffIlrRepository;
            _standardService = staffService;
        }

        public async Task<StaffSearchResult> Handle(StaffSearchRequest request, CancellationToken cancellationToken)
        {
            var pageSize = 10;

            if (string.IsNullOrWhiteSpace(request.SearchQuery))
                return new StaffSearchResult
                {
                    EndpointAssessorOrganisationId = String.Empty,
                    StaffSearchItems =
                        new PaginatedList<StaffSearchItems>(new List<StaffSearchItems>(), 0, request.Page, pageSize)
                };

            var searchResult = await Search(request);
            var totalRecordCount = searchResult.TotalCount;

            var displayEpao = false;
            if (searchResult.TotalCount == 0)
            {
                totalRecordCount = await _staffIlrRepository.SearchForLearnerByNameCount(request.SearchQuery);
                searchResult.PageOfResults = await _staffIlrRepository.SearchForLearnerByName(request.SearchQuery, request.Page, pageSize);
            }
            else
            {
                displayEpao = searchResult.DisplayEpao;
            }

            _logger.LogInformation(searchResult.PageOfResults.Any() ? LoggingConstants.SearchSuccess : LoggingConstants.SearchFailure);

            var searchResults = Mapper.Map<List<StaffSearchItems>>(searchResult.PageOfResults);

            searchResults = MatchUpExistingCompletedStandards(searchResults);
            searchResults = await PopulateStandards(searchResults, _standardService, _logger);

            return new StaffSearchResult
            {
                DisplayEpao = displayEpao,
                EndpointAssessorOrganisationId = displayEpao && searchResults.Count > 0 ? searchResults.First().EndpointAssessorOrganisationId : string.Empty,
                StaffSearchItems = new PaginatedList<StaffSearchItems>(searchResults, totalRecordCount, request.Page, pageSize)
            };
        }

        private async Task<StaffReposSearchResult> Search(StaffSearchRequest request)
        {
            if (SearchStringIsAnEpaOrgId(request))
            {                
                var sr = await _staffIlrRepository.SearchForLearnerByEpaOrgId(request);
                sr.DisplayEpao = true;
                return sr;
            }

            var pageSize = 10;
            if (SearchStringIsAUln(request, out var uln))           
            {
                var sr = new StaffReposSearchResult
                {
                    PageOfResults = await _staffIlrRepository.SearchForLearnerByUln(uln, request.Page, pageSize),
                    TotalCount = await _staffIlrRepository.SearchForLearnerByUlnCount(uln)
                };
                return sr;
            }

            if (SearchStringIsACertificateReference(request))
            {
                var sr = new StaffReposSearchResult
                {
                    DisplayEpao = true,
                    PageOfResults = await _staffIlrRepository.SearchForLearnerByCertificateReference(request.SearchQuery)
                };
                sr.TotalCount = sr.PageOfResults.Count();
                return sr;
            }

            return new StaffReposSearchResult() { PageOfResults = new List<Ilr>(), TotalCount = 0 };
        }

        private static bool SearchStringIsAnEpaOrgId(StaffSearchRequest request)
        {
            var regex = new Regex(@"\b(?i)(epa)[0-9]{4}\b");
            return regex.IsMatch(request.SearchQuery);
        }

        private static bool SearchStringIsACertificateReference(StaffSearchRequest request)
        {
            return request.SearchQuery.Length == 8 && long.TryParse(request.SearchQuery, out var certRef);
        }

        private static bool SearchStringIsAUln(StaffSearchRequest request, out long returningUln)
        {
            long uln = 0;
            var searchStringIsAUln = request.SearchQuery.Length == 10 && long.TryParse(request.SearchQuery, out uln);
            returningUln = uln;
            return searchStringIsAUln;
        }


        private List<StaffSearchItems> MatchUpExistingCompletedStandards(List<StaffSearchItems> searchResults)
        {
            _logger.LogInformation("MatchUpExistingCompletedStandards Before Get Certificates for uln from db");
            var completedCertificates = _staffCertificateRepository.GetCertificatesFor(searchResults.Select(r => r.Uln).ToArray()).Result;
            _logger.LogInformation("MatchUpExistingCompletedStandards After Get Certificates for uln from db");

            foreach (var searchResult in searchResults)
            {
                var certificate = completedCertificates.SingleOrDefault(s => s.StandardCode == searchResult.StandardCode && s.Uln == searchResult.Uln);
                if (certificate == null) continue;

                searchResult.CertificateReference = certificate.CertificateReference;
                searchResult.CertificateStatus = certificate.Status;
                searchResult.GivenNames = certificate.GivenNames;
                searchResult.FamilyName = certificate.FamilyName;
                searchResult.EndpointAssessorOrganisationId = certificate.EndPointAssessorOrganisationId;
                searchResult.Standard = certificate.StandardName;

                if (searchResult.LastUpdatedAt == null)
                {
                    searchResult.LastUpdatedAt = certificate.LastUpdatedAt?.UtcToTimeZoneTime();
                }
            }
            return searchResults;
        }

        private async Task<List<StaffSearchItems>> PopulateStandards(List<StaffSearchItems> searchResults, IStandardService standardService, ILogger<SearchHandler> logger)
        {
            var allStandards = await standardService.GetAllStandards();

            foreach (var searchResult in searchResults.Where(sr => string.IsNullOrEmpty(sr.Standard)))
            {
                var standard = allStandards.SingleOrDefault(s => s.StandardId == searchResult.StandardCode) ?? await standardService.GetStandard(searchResult.StandardCode);

                if (standard != null)
                {
                    searchResult.Standard = standard.Title;
                }
            }

            return searchResults;
        }
    }
}