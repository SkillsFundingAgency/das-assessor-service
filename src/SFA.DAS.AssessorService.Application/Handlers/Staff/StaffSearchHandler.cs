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
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;

namespace SFA.DAS.AssessorService.Application.Handlers.Search
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
            var ilrResults = _ilrRepository.Search(request.SearchQuery);


            _logger.LogInformation(ilrResults.Any() ? LoggingConstants.SearchSuccess : LoggingConstants.SearchFailure);

            var searchResults = Mapper.Map<List<SearchResult>>(ilrResults);

            searchResults = MatchUpExistingCompletedStandards(searchResults, request, _certificateRepository, _contactRepository, _logger)
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

         public static List<SearchResult> MatchUpExistingCompletedStandards(List<SearchResult> searchResults, SearchQuery request, ICertificateRepository certificateRepository, IContactQueryRepository contactRepository, ILogger<SearchHandler> logger)
        {
            logger.LogInformation("MatchUpExistingCompletedStandards Before Get Certificates for uln from db");
            var completedCertificates = certificateRepository.GetCompletedCertificatesFor(request.Uln).Result;
            logger.LogInformation("MatchUpExistingCompletedStandards After Get Certificates for uln from db");
            foreach (var searchResult in searchResults.Where(r => completedCertificates.Select(s => s.StandardCode).Contains(r.StdCode)))
            {
                var certificate = completedCertificates.Single(s => s.StandardCode == searchResult.StdCode);
                var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                searchResult.CertificateReference = certificate.CertificateReference;
                searchResult.LearnStartDate = certificateData.LearningStartDate == DateTime.MinValue ? null : new DateTime?(certificateData.LearningStartDate) ;

                var certificateLogs = certificateRepository.GetCertificateLogsFor(certificate.Id).Result;
                logger.LogInformation("MatchUpExistingCompletedStandards After GetCertificateLogsFor");
                var submittedLogEntry = certificateLogs.FirstOrDefault(l => l.Status == CertificateStatus.Submitted);
                if (submittedLogEntry == null) continue;
                
              
              
            }

            return searchResults;
        }
    }
}