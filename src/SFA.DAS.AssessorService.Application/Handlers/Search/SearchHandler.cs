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
using SFA.DAS.AssessorService.ExternalApis.Services;
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
        private readonly IStandardService _standardService;
        private Dictionary<char, char[]> _alternates;

        public SearchHandler(IAssessmentOrgsApiClient assessmentOrgsApiClient, IOrganisationQueryRepository organisationRepository, 
            IIlrRepository ilrRepository, ICertificateRepository certificateRepository, ILogger<SearchHandler> logger, IContactQueryRepository contactRepository, IStandardService standardService)
        {
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
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
            _logger.LogInformation($"Search for surname: {request.Surname} uln: {request.Uln} made by {request.EpaOrgId}");

            var thisEpao = await _organisationRepository.Get(request.EpaOrgId);

            if (thisEpao == null)
            {
                _logger.LogInformation(LoggingConstants.SearchFailure);
                return new List<SearchResult>();
            }

            var intStandards = await GetEpaoStandards(thisEpao);

            var ilrResults = await _ilrRepository.SearchForLearnerByUln(request.Uln);

            var likedSurname = request.Surname.Replace(" ", "");

            var listOfIlrResults = ilrResults?.ToList();
            if (request.IsPrivatelyFunded && (listOfIlrResults == null || (!listOfIlrResults.Any())))
            {
                //Learner not in ILR so try to create an in memory record with details from found certificate and request information
                listOfIlrResults = new List<Ilr> { new Ilr { Uln = request.Uln, EpaOrgId = request.EpaOrgId, FamilyNameForSearch = request.Surname, FamilyName = request.Surname } };
                likedSurname = DealWithSpecialCharactersAndSpaces(request, likedSurname, listOfIlrResults);
                var certificate=
                    await _certificateRepository.GetCertificateByOrgIdLastname(request.Uln, request.EpaOrgId, likedSurname); 
                if (certificate == null) 
                {
                    //Now check if exists for uln and surname without considering org
                    certificate = await _certificateRepository.GetCertificateByUlnLastname(request.Uln, likedSurname);
                    if (certificate != null)
                    {
                        if (intStandards?.Contains(certificate.StandardCode) ?? false)
                        {
                            var standard = await  _standardService.GetStandard(certificate.StandardCode);
                            return new List<SearchResult>
                            {
                                new SearchResult {UlnAlreadyExits = true,FamilyName = likedSurname, Uln = request.Uln, IsPrivatelyFunded = true, Standard = standard?.Title,Level = standard?.StandardData.Level.GetValueOrDefault()??0}
                            };
                        }
                        return new List<SearchResult> { new SearchResult { UlnAlreadyExits = true, Uln = request.Uln, IsPrivatelyFunded = true, IsNoMatchingFamilyName = true } };
                    }

                    //If we got here then certifcate does not exist with uln and surename so
                    //lastly check if there is a certificate that exist with the given uln only disregarding org and surname
                    var certificateExist = await _certificateRepository.CertifciateExistsForUln(request.Uln);
                    return certificateExist
                        ? new List<SearchResult> {new SearchResult {UlnAlreadyExits = true, Uln = request.Uln, IsPrivatelyFunded = true, IsNoMatchingFamilyName = true } }
                        : new List<SearchResult>();
                }
                
                //We found the certifate, check if standard in certificate exists in standards registered by calling org
                if (intStandards?.Contains(certificate.StandardCode)??false)
                    listOfIlrResults[0].StdCode = certificate.StandardCode;
            }
            else
            {
                likedSurname = DealWithSpecialCharactersAndSpaces(request, likedSurname, listOfIlrResults);
            }


            ilrResults = listOfIlrResults?.Where(r =>(
                r.EpaOrgId == thisEpao.EndPointAssessorOrganisationId ||
                (r.EpaOrgId != thisEpao.EndPointAssessorOrganisationId && intStandards.Contains(r.StdCode)))
            && string.Equals(r.FamilyNameForSearch.Trim(), likedSurname.Trim(), StringComparison.CurrentCultureIgnoreCase)).ToList();

            //If privately funded and uln found in ilr but due to the above check the result was empty then set uln exist flag
            if (request.IsPrivatelyFunded && ilrResults != null && !ilrResults.Any())
            {
              return  new List<SearchResult> { new SearchResult{UlnAlreadyExits = true, Uln = request.Uln , IsPrivatelyFunded = true, IsNoMatchingFamilyName = true } };
            }

            _logger.LogInformation((ilrResults != null && ilrResults.Any())? LoggingConstants.SearchSuccess : LoggingConstants.SearchFailure);

            var searchResults = Mapper.Map<List<SearchResult>>(ilrResults)
                .MatchUpExistingCompletedStandards(request, _certificateRepository, _contactRepository, _logger)
                .PopulateStandards(_standardService, _logger);


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