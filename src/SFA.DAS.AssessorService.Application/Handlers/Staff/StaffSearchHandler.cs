using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Search;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class StaffSearchHandler : IRequestHandler<StaffSearchRequest, List<StaffSearchResult>>
    {
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly IStaffCertificateRepository _staffCertificateRepository;
        private readonly ILogger<SearchHandler> _logger;
        private readonly IContactQueryRepository _contactRepository;
        private Dictionary<char, char[]> _alternates;

        public StaffSearchHandler(IAssessmentOrgsApiClient assessmentOrgsApiClient, IOrganisationQueryRepository organisationRepository,
            IIlrRepository ilrRepository, IStaffCertificateRepository staffCertificateRepository, ILogger<SearchHandler> logger, IContactQueryRepository contactRepository)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _organisationRepository = organisationRepository;
            _ilrRepository = ilrRepository;
            _staffCertificateRepository = staffCertificateRepository;
            _logger = logger;
            _contactRepository = contactRepository;

            BuildAlternates();

        }

        private void BuildAlternates()
        {
            _alternates = new Dictionary<char, char[]>
            {
                {'\'', new[] {'`', '’', '\''}},
                {'’', new[] {'`', '\'','’'}},
                {'`', new[] {'\'', '’', '`'}},

                {'–', new[] {'-', '–'}},
                {'-', new[] {'–', '-'}}
            };
        }

        public async Task<List<StaffSearchResult>> Handle(StaffSearchRequest request, CancellationToken cancellationToken)
        {
            var ilrResults = await Search(request);

            _logger.LogInformation(ilrResults.Any() ? LoggingConstants.SearchSuccess : LoggingConstants.SearchFailure);

            var searchResults = Mapper.Map<List<StaffSearchResult>>(ilrResults);

            searchResults = MatchUpExistingCompletedStandards(searchResults);
            searchResults = PopulateStandards(searchResults, _assessmentOrgsApiClient, _logger);

            return searchResults;
        }

        private async Task<IEnumerable<Ilr>> Search(StaffSearchRequest request)
        {
            // Naive decision on what is being searched.
            if (request.SearchQuery.Length == 10 && long.TryParse(request.SearchQuery, out var uln))
            {
                // Search string is a long of 10 length so must be a uln.
                return await _ilrRepository.SearchForLearnerByUln(uln);
            }

            if (request.SearchQuery.Length == 8 && long.TryParse(request.SearchQuery, out var certRef))
            {
                // Search string is 8 chars and is a valid long so must be a CertificateReference
                return await _ilrRepository.SearchForLearnerByCertificateReference(request.SearchQuery);
            }

            // None of the above, search on Surname / firstname.
            return await _ilrRepository.SearchForLearnerByName(request.SearchQuery);   
        }

        private List<int> ConvertStandardsToListOfInts(IEnumerable<StandardOrganisationSummary> theStandardsThisEpaoProvides)
        {
            var list = new List<int>();
            foreach (var standardSummary in theStandardsThisEpaoProvides)
            {
                if (int.TryParse(standardSummary.StandardCode, out var stdCode))
                {
                    list.Add(stdCode);
                }
            }

            return list;
        }

        private char[] SpecialCharactersInSurname(string surname)
        {
            return _alternates.Where(kvp => surname.Contains(kvp.Key)).Select(kvp => kvp.Key).ToArray();
        }

        private List<StaffSearchResult> MatchUpExistingCompletedStandards(List<StaffSearchResult> searchResults)
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
                searchResult.LastUpdatedAt = certificate.LastUpdatedAt?.ToLocalTime();
                //searchResult.LearnStartDate = certificate.LearningStartDate == DateTime.MinValue ? null : new DateTime?(certificate.LearningStartDate);
                //searchResult.ShowExtraInfo = true;
                //searchResult.OverallGrade = certificate.OverallGrade;
                //searchResult.SubmittedBy = certificate.SubmittedBy; // This needs to be contact real name
                //searchResult.SubmittedAt = certificate.SubmittedAt.ToLocalTime(); // This needs to be local time 
                //searchResult.AchDate = certificate.AchievementDate;
            }
            return searchResults;
        }

        private List<StaffSearchResult> PopulateStandards(List<StaffSearchResult> searchResults, IAssessmentOrgsApiClient assessmentOrgsApiClient, ILogger<SearchHandler> logger)
        {
            var allStandards = assessmentOrgsApiClient.GetAllStandards().Result;

            foreach (var searchResult in searchResults)
            {
                var standard = allStandards.SingleOrDefault(s => s.Id == searchResult.StandardCode);
                if (standard == null)
                {
                    standard = assessmentOrgsApiClient.GetStandard(searchResult.StandardCode).Result;
                }
                searchResult.Standard = standard.Title;
                //searchResult.Level = standard.Level;
            }

            return searchResults;
        }
    }
}