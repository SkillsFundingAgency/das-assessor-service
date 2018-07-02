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
    public class StaffSearchHandler : IRequestHandler<StaffSearchRequest, List<SearchResult>>
    {
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<SearchHandler> _logger;
        private readonly IContactQueryRepository _contactRepository;
        private Dictionary<char, char[]> _alternates;

        public StaffSearchHandler(IAssessmentOrgsApiClient assessmentOrgsApiClient, IOrganisationQueryRepository organisationRepository,
            IIlrRepository ilrRepository, ICertificateRepository certificateRepository, ILogger<SearchHandler> logger, IContactQueryRepository contactRepository)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _organisationRepository = organisationRepository;
            _ilrRepository = ilrRepository;
            _certificateRepository = certificateRepository;
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

        public async Task<List<SearchResult>> Handle(StaffSearchRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Ilr> ilrResults;

            // Naive decision on what is being searched.
            if (request.SearchQuery.Length == 10 && long.TryParse(request.SearchQuery, out var uln))
            {
                // Search string is a long of 10 length so must be a uln.
                ilrResults = await _ilrRepository.SearchForLearnerByUln(uln);
            }

            if (request.SearchQuery.Length == 8 && long.TryParse(request.SearchQuery, out var certRef))
            {
                // Search string is 8 chars and is a valid long so must be a CertificateReference
                ilrResults = await _ilrRepository.SearchForLearnerByCertificateReference(request.SearchQuery);
            }

            // None of the above, search on Surname / firstname.
            ilrResults = await _ilrRepository.SearchForLearnerByName(request.SearchQuery);

            _logger.LogInformation(ilrResults.Any() ? LoggingConstants.SearchSuccess : LoggingConstants.SearchFailure);

            var searchResults = Mapper.Map<List<SearchResult>>(ilrResults);

            searchResults = MatchUpExistingCompletedStandards(searchResults)
                .PopulateStandards(_assessmentOrgsApiClient, _logger);

            return searchResults;
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

        private List<SearchResult> MatchUpExistingCompletedStandards(List<SearchResult> searchResults)
        {
            foreach (var searchResult in searchResults)
            {
                _logger.LogInformation("MatchUpExistingCompletedStandards Before Get Certificates for uln from db");
                var completedCertificates = _certificateRepository.GetCompletedCertificatesFor(searchResult.Uln).Result;
                _logger.LogInformation("MatchUpExistingCompletedStandards After Get Certificates for uln from db");

                var certificate = completedCertificates.SingleOrDefault(s => s.StandardCode == searchResult.StdCode);
                if (certificate == null) continue;

                var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                searchResult.CertificateReference = certificate.CertificateReference;
                searchResult.LearnStartDate = certificateData.LearningStartDate == DateTime.MinValue ? null : new DateTime?(certificateData.LearningStartDate);

                var certificateLogs = _certificateRepository.GetCertificateLogsFor(certificate.Id).Result;
                _logger.LogInformation("MatchUpExistingCompletedStandards After GetCertificateLogsFor");
                var submittedLogEntry = certificateLogs.FirstOrDefault(l => l.Status == CertificateStatus.Submitted);
                if (submittedLogEntry == null) continue;

                var submittingContact = _contactRepository.GetContact(submittedLogEntry.Username).Result;
                _logger.LogInformation("MatchUpExistingCompletedStandards After GetContact");

                searchResult.ShowExtraInfo = true;
                searchResult.OverallGrade = certificateData.OverallGrade;
                searchResult.SubmittedBy = submittingContact.DisplayName; // This needs to be contact real name
                searchResult.SubmittedAt = submittedLogEntry.EventTime.ToLocalTime(); // This needs to be local time 
                searchResult.AchDate = certificateData.AchievementDate;
            }
            return searchResults;
        }
    }
}