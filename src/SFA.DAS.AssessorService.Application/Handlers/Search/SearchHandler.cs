using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.SubmissionEvents;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.Handlers.Search
{
    public class SearchHandler : IRequestHandler<SearchQuery, List<SearchResult>>
    {
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<SearchHandler> _logger;
        private readonly IContactQueryRepository _contactRepository;
        private Dictionary<char, char[]> _alternates;
        private readonly ISubmissionEventProviderApiClient _subProviderEventsApiClient;
     
        public SearchHandler(IAssessmentOrgsApiClient assessmentOrgsApiClient,
            IOrganisationQueryRepository organisationRepository,
            IIlrRepository ilrRepository,
            ICertificateRepository certificateRepository, ILogger<SearchHandler> logger,
            IContactQueryRepository contactRepository,
            ISubmissionEventProviderApiClient subProviderEventsApiClient)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _organisationRepository = organisationRepository;
            _subProviderEventsApiClient = subProviderEventsApiClient;
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
                {'’', new[] {'`', '\'', '’'}},
                {'`', new[] {'\'', '’', '`'}},

                {'–', new[] {'-', '–'}},
                {'-', new[] {'–', '-'}}
            };
        }

        public async Task<List<SearchResult>> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Search for surname: {request.Surname} uln: {request.Uln} made by {request.UkPrn}");
            var likedSurname = request.Surname.Replace(" ", "");
           likedSurname = DealWithSpecialCharactersAndSpaces(request, likedSurname, new List<Ilr>());

            //If standard code is provided along side of uln and likedsurname
            if (!string.IsNullOrEmpty(request.StandardCode))
            {
               var foundMatch = await RetrieveSearchResultFromMatchedCertificate(request, likedSurname);
               return foundMatch;
            }

            //Update Ilr cache 
            await UpdateIlrsCache(request, likedSurname);
            //Retrieve from updated Ilr cache
            var uncleanedCachedData = await _ilrRepository.SearchForLearnerByUlnAndFamilyName(request.Uln, likedSurname);
            var cachedData = uncleanedCachedData.ToList();
            DealWithSpecialCharactersAndSpaces(request, likedSurname, cachedData);

            //Filter by Compstatus
            var ilrResultsFilteredByCompStatus =
            cachedData.Where(r => r.CompletionStatus == CompletionStatus.Continuing || 
                                  r.CompletionStatus == CompletionStatus.Completed);

            //Check validity of EpaOrgId and Filter
            var thisEpao = await _organisationRepository.GetByUkPrn(request.UkPrn);
            var intStandards = await GetEpaoStandards(thisEpao);

            var ilrResults = ilrResultsFilteredByCompStatus.Where(r => (
                                                   r.EpaOrgId == thisEpao.EndPointAssessorOrganisationId ||
                                                   (r.EpaOrgId != thisEpao.EndPointAssessorOrganisationId &&
                                                    intStandards.Contains(r.StdCode)))
                                               && string.Equals(r.FamilyNameForSearch.Trim(), likedSurname.Trim(),
                                                   StringComparison.CurrentCultureIgnoreCase)).ToList();

          
            _logger.LogInformation(ilrResults.Any() ? LoggingConstants.SearchSuccess : LoggingConstants.SearchFailure);

            var searchResults = Mapper.Map<List<SearchResult>>(ilrResults)
                .MatchUpExistingCompletedStandards(request, _certificateRepository, _contactRepository, _logger)
                .PopulateStandards(_assessmentOrgsApiClient, _logger);

            await _ilrRepository.StoreSearchLog(new SearchLog()
            {
                NumberOfResults = searchResults.Count,
                SearchTime = DateTime.UtcNow,
                Surname = request.Surname,
                Uln = request.Uln,
                Username = request.Username
            });

            return searchResults;
        }


        private async Task<IEnumerable<Ilr>> UpdateIlrsCache(SearchQuery request, string likedSurname)
        {
            //The ULN and FamilyName should be matched to the ILR cache
            var ilrResults = await _ilrRepository.SearchForLearnerByUlnAndFamilyName(request.Uln, likedSurname);
            var dataForIlrsCache = ilrResults.ToList();
            if (dataForIlrsCache.Any())
            {
                //check if Eventid is Null or UpdateAt is earlier then today, then refresh cache
                foreach (var ilrResult in dataForIlrsCache)
                {
                    if (ilrResult.EventId == null ||
                        (ilrResult.UpdatedAt != null && ilrResult.UpdatedAt?.Date < DateTime.Today))
                    {
                        var submissionEventsList = ilrResult.EventId == null
                            ? await _subProviderEventsApiClient.GetLatestLearnerEventForStandards(ilrResult.Uln)
                            : await _subProviderEventsApiClient.GetLatestLearnerEventForStandards(ilrResult.Uln,
                                ilrResult.EventId.Value);

                        foreach (var submissionEvent in submissionEventsList)
                        {
                            if (submissionEvent.StandardCode != ilrResult.StdCode) continue;
                            //ILR cache record needs updating
                            await _ilrRepository.RefreshFromSubmissionEventData(ilrResult.Id, submissionEvent);
                            await _ilrRepository.MarkAllUpToDate(ilrResult.Uln);
                        }
                    }
                }
            }

            return dataForIlrsCache;
        }


        private async Task<List<SearchResult>> RetrieveSearchResultFromMatchedCertificate(SearchQuery request,
            string likedSurname)
        {
            var listOfSearchResults = new List<SearchResult>();
            //The ULN and FamilyName with  Standard Code  matched to existing Certificate then return details from certificate
            var certificates = await _certificateRepository.GetCertificates(request.Uln, likedSurname,
                Int32.Parse(request.StandardCode));
            if (certificates.Any() && certificates.Count == 1)
            {
                var ilr = new Ilr {StdCode = certificates[0].StandardCode};
                var result = Mapper.Map<SearchResult>(ilr)
                    .MatchUpFoundCertificate(request, certificates, _assessmentOrgsApiClient,
                        _contactRepository,
                        _certificateRepository, _logger);

                _logger.LogInformation(certificates.Any()
                    ? LoggingConstants.SearchSuccess
                    : LoggingConstants.SearchFailure);

                listOfSearchResults.Add(result);
            }

            return listOfSearchResults;
        }

        private async Task<List<int>> GetEpaoStandards(Organisation thisEpao)
        {
            //Todo: get from our database don't need to go to external API
            var theStandardsThisEpaoProvides = await _assessmentOrgsApiClient.FindAllStandardsByOrganisationIdAsync(thisEpao
                .EndPointAssessorOrganisationId);

            var intStandards = ConvertStandardsToListOfInts(theStandardsThisEpaoProvides);
            return intStandards;
        }

        private string DealWithSpecialCharactersAndSpaces(SearchQuery request, string likedSurname, IEnumerable<Ilr> ilrResults)
        {
            var enumerable = ilrResults as Ilr[] ?? ilrResults.ToArray();
            foreach (var ilrResult in enumerable)
            {
                ilrResult.FamilyNameForSearch = ilrResult.FamilyName.Replace(" ", "");
            }

            var specialCharacters = SpecialCharactersInSurname(request.Surname);
            if (specialCharacters.Length > 0)
            {
                foreach (var specialCharacter in specialCharacters)
                {
                    likedSurname = likedSurname.Replace(specialCharacter.ToString(), "");
                }

                foreach (var ilrResult in enumerable)
                {
                    foreach (var specialCharacter in specialCharacters)
                    {
                        foreach (var alternate in _alternates[specialCharacter])
                        {
                            ilrResult.FamilyNameForSearch = ilrResult.FamilyNameForSearch.Replace(alternate.ToString(), "");
                        }
                    }
                }
            }

            return likedSurname;
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
    }
}