using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;

namespace SFA.DAS.AssessorService.Application.Handlers.Search
{
    public class SearchHandler : IRequestHandler<SearchQuery, List<SearchResult>>
    {
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<SearchHandler> _logger;
        private Dictionary<char, char[]> _alternates;

        public SearchHandler(IAssessmentOrgsApiClient assessmentOrgsApiClient, IOrganisationQueryRepository organisationRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, ILogger<SearchHandler> logger)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _organisationRepository = organisationRepository;
            _ilrRepository = ilrRepository;
            _certificateRepository = certificateRepository;
            _logger = logger;
            
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

            var theStandardsThisEpaoProvides = await _assessmentOrgsApiClient.FindAllStandardsByOrganisationIdAsync(thisEpao
                .EndPointAssessorOrganisationId);

            var intStandards = ConvertStandardsToListOfInts(theStandardsThisEpaoProvides);
            
            var specialCharacters = SpecialCharactersInSurname(request.Surname);
            IEnumerable<Ilr> ilrResults;
            if (specialCharacters.Length > 0)
            {
                var likedSurname = request.Surname;
                foreach (var specialCharacter in specialCharacters)
                {
                    likedSurname = likedSurname.Replace(specialCharacter, '_');
                }
                
                ilrResults = await _ilrRepository.SearchForLearnerLike(new SearchRequest
                {
                    FamilyName = likedSurname,
                    Uln = request.Uln
                });
            }
            else
            {
                ilrResults = await _ilrRepository.SearchForLearner(new SearchRequest
                {
                    FamilyName = request.Surname,
                    Uln = request.Uln
                });
            }

            ilrResults = ilrResults.Where(r =>
                r.EpaOrgId == thisEpao.EndPointAssessorOrganisationId ||
                (r.EpaOrgId != thisEpao.EndPointAssessorOrganisationId && intStandards.Contains(r.StdCode))).ToList();
            

            _logger.LogInformation(ilrResults.Any() ? LoggingConstants.SearchSuccess : LoggingConstants.SearchFailure);

            var searchResults = Mapper.Map<List<SearchResult>>(ilrResults)
                .MatchUpExistingCompletedStandards(request, _certificateRepository)
                .PopulateStandards(_assessmentOrgsApiClient);

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
    }
}