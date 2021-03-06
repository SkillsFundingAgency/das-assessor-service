﻿using AutoMapper;
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
    public class SearchHandler : IRequestHandler<SearchQuery, List<SearchResult>>
    {
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly IIlrRepository _ilrRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<SearchHandler> _logger;
        private readonly IContactQueryRepository _contactRepository;
        private readonly IStandardService _standardService;
        private Dictionary<char, char[]> _alternates;

        public SearchHandler(IOrganisationQueryRepository organisationRepository, 
            IIlrRepository ilrRepository, ICertificateRepository certificateRepository, ILogger<SearchHandler> logger, IContactQueryRepository contactRepository, IStandardService standardService)
        {
            _organisationRepository = organisationRepository;
            _ilrRepository = ilrRepository;
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

        public async Task<List<SearchResult>> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            var searchResults = await Search(request, cancellationToken);

            await _ilrRepository.StoreSearchLog(new SearchLog()
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

        private async Task<List<SearchResult>> Search(SearchQuery request, CancellationToken cancellationToken)
        { 
            _logger.LogInformation($"Search for surname: {request.Surname} uln: {request.Uln} made by {request.EpaOrgId}");

            var thisEpao = await _organisationRepository.Get(request.EpaOrgId);
            if (thisEpao == null)
            {
                _logger.LogInformation($"{LoggingConstants.SearchFailure} - Invalid EpaOrgId", request.EpaOrgId);
                return new List<SearchResult>();
            }

            var approvedStandards = await GetEpaoApprovedStandardsWithAtLeastOneVersion(thisEpao);

            var ilrResults = await _ilrRepository.SearchForLearnerByUln(request.Uln);

            var likedSurname = request.Surname.Replace(" ", "");

            var listOfIlrResults = ilrResults?.ToList();

            likedSurname = DealWithSpecialCharactersAndSpaces(request, likedSurname, listOfIlrResults);

            ilrResults = listOfIlrResults?.Where(r => approvedStandards.Contains(r.StdCode) &&
                string.Equals(r.FamilyNameForSearch.Trim(), likedSurname.Trim(), StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            _logger.LogInformation((ilrResults != null && ilrResults.Any())? LoggingConstants.SearchSuccess : LoggingConstants.SearchFailure);

            var searchResults = Mapper.Map<List<SearchResult>>(ilrResults)
                .MatchUpExistingCompletedStandards(request, _certificateRepository, _contactRepository, _organisationRepository, _logger)
                .PopulateStandards(_standardService, _logger);

            return searchResults;
        }

        private async Task<IEnumerable<int>> GetEpaoApprovedStandardsWithAtLeastOneVersion(Organisation thisEpao)
        {
            var approvedStandardVersions = await _standardService.GetEPAORegisteredStandardVersions(thisEpao.EndPointAssessorOrganisationId);

            var approvedStandardCodes = approvedStandardVersions.Select(standard => standard.LarsCode).Distinct();
          
            return approvedStandardCodes;
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


        private char[] SpecialCharactersInSurname(string surname)
        {
            return _alternates.Where(kvp => surname.Contains(kvp.Key)).Select(kvp => kvp.Key).ToArray();
        }
    }
}