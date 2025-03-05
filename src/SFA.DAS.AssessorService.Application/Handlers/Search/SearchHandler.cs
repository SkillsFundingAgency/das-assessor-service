using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;
using SearchData = SFA.DAS.AssessorService.Domain.Entities.SearchData;

namespace SFA.DAS.AssessorService.Application.Handlers.Search
{
    public class SearchHandler : BaseHandler, IRequestHandler<LearnerSearchRequest, List<LearnerSearchResponse>>
    {
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly ILearnerRepository _learnerRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<SearchHandler> _logger;
        private readonly IContactQueryRepository _contactRepository;
        private readonly IStandardService _standardService;
        private Dictionary<char, char[]> _alternates;

        public SearchHandler(IOrganisationQueryRepository organisationRepository,
            ILearnerRepository learnerRepository, ICertificateRepository certificateRepository, ILogger<SearchHandler> logger, IContactQueryRepository contactRepository, IStandardService standardService, IMapper mapper)
            :base(mapper)
        {
            _organisationRepository = organisationRepository;
            _learnerRepository = learnerRepository;
            _certificateRepository = certificateRepository;
            _logger = logger;
            _contactRepository = contactRepository;
            _standardService = standardService;

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

        public async Task<List<LearnerSearchResponse>> Handle(LearnerSearchRequest request, CancellationToken cancellationToken)
        {
            var searchResults = await Search(request, cancellationToken);

            await _learnerRepository.StoreSearchLog(new SearchLog()
            {
                NumberOfResults = searchResults.Count,
                SearchTime = DateTime.UtcNow,
                SearchData = new SearchData(),
                Surname = request.Surname,
                Uln = request.Uln,
                Username = request.Username
            });

            return searchResults;
        }

        private async Task<List<LearnerSearchResponse>> Search(LearnerSearchRequest request, CancellationToken cancellationToken)
        { 
            _logger.LogInformation($"Search for surname: {request.Surname} uln: {request.Uln} made by {request.EpaOrgId}");

            var thisEpao = await _organisationRepository.Get(request.EpaOrgId);
            if (thisEpao == null)
            {
                _logger.LogInformation($"{LoggingConstants.SearchFailure} - Invalid EpaOrgId", request.EpaOrgId);
                return new List<LearnerSearchResponse>();
            }

            var approvedStandards = await GetEpaoApprovedStandardsWithAtLeastOneVersion(thisEpao);

            var learnerResults = await _learnerRepository.SearchForLearnerByUln(request.Uln);

            var likedSurname = request.Surname.Replace(" ", "");

            var listOfLearnerResults = learnerResults?.ToList();

            likedSurname = DealWithSpecialCharactersAndSpaces(request, likedSurname, listOfLearnerResults);

            learnerResults = listOfLearnerResults?.Where(r => approvedStandards.Contains(r.StdCode) &&
                string.Equals(r.FamilyNameForSearch.Trim(), likedSurname.Trim(), StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            _logger.LogInformation((learnerResults != null && learnerResults.Any())? LoggingConstants.SearchSuccess : LoggingConstants.SearchFailure);

            var searchResults = _mapper.Map<List<LearnerSearchResponse>>(learnerResults)
                .MatchUpExistingCompletedStandards(request, likedSurname, approvedStandards, _certificateRepository, _contactRepository, _organisationRepository, _logger)
                .PopulateStandards(_standardService, _logger);

            return searchResults;
        }

        private async Task<IEnumerable<int>> GetEpaoApprovedStandardsWithAtLeastOneVersion(Organisation thisEpao)
        {
            var approvedStandardVersions = await _standardService.GetEPAORegisteredStandardVersions(thisEpao.EndPointAssessorOrganisationId);

            var approvedStandardCodes = approvedStandardVersions.Select(standard => standard.LarsCode).Distinct();
          
            return approvedStandardCodes;
        }

        private string DealWithSpecialCharactersAndSpaces(LearnerSearchRequest request, string likedSurname, IEnumerable<Domain.Entities.Learner> learnerResults)
        {
            foreach (var learnerResult in learnerResults)
            {
                learnerResult.FamilyNameForSearch = learnerResult.FamilyName.Replace(" ", "");
            }

            var specialCharacters = SpecialCharactersInSurname(request.Surname);
            if (specialCharacters.Length > 0)
            {
                foreach (var specialCharacter in specialCharacters)
                {
                    likedSurname = likedSurname.Replace(specialCharacter.ToString(), "");
                }

                foreach (var learnerResult in learnerResults)
                {
                    foreach (var specialCharacter in specialCharacters)
                    {
                        foreach (var alternate in _alternates[specialCharacter])
                        {
                            learnerResult.FamilyNameForSearch = learnerResult.FamilyNameForSearch.Replace(alternate.ToString(), "");
                        }
                    }
                }
            }

            return likedSurname;
        }


        private char[] SpecialCharactersInSurname(string surname)
        {
            return _alternates.Where(kvp => surname.Contains(kvp.Key)).Select(kvp => kvp.Key).ToArray();
        }
    }
}