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
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
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

        public SearchHandler(IAssessmentOrgsApiClient assessmentOrgsApiClient, IOrganisationQueryRepository organisationRepository, 
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

        public async Task<List<SearchResult>> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Search for surname: {request.Surname} uln: {request.Uln} made by {request.UkPrn}");

            var thisEpao = await _organisationRepository.GetByUkPrn(request.UkPrn);

            var intStandards = await GetEpaoStandards(thisEpao);

            var ilrResults = await _ilrRepository.SearchForLearnerByUln(request.Uln);

            var likedSurname = request.Surname.Replace(" ","");

            likedSurname = DealWithSpecialCharactersAndSpaces(request, likedSurname, ilrResults);

            ilrResults = ilrResults.Where(r =>(
                r.EpaOrgId == thisEpao.EndPointAssessorOrganisationId ||
                (r.EpaOrgId != thisEpao.EndPointAssessorOrganisationId && intStandards.Contains(r.StdCode)))
            && string.Equals(r.FamilyNameForSearch.Trim(), likedSurname.Trim(), StringComparison.CurrentCultureIgnoreCase)).ToList();
            

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

        private async Task<List<int>> GetEpaoStandards(Organisation thisEpao)
        {
            var theStandardsThisEpaoProvides = await _assessmentOrgsApiClient.FindAllStandardsByOrganisationIdAsync(thisEpao
                .EndPointAssessorOrganisationId);

            var intStandards = ConvertStandardsToListOfInts(theStandardsThisEpaoProvides);
            return intStandards;
        }

        private string DealWithSpecialCharactersAndSpaces(SearchQuery request, string likedSurname, IEnumerable<Ilr> ilrResults)
        {
            foreach (var ilrResult in ilrResults)
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

                foreach (var ilrResult in ilrResults)
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