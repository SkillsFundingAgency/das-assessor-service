using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Orchestrators.Search
{
    public class SearchOrchestrator : ISearchOrchestrator
    {
        private readonly ILogger<SearchController> _logger;
        private readonly IMapper _mapper;
        private readonly ISearchApiClient _searchApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public SearchOrchestrator(ILogger<SearchController> logger, IMapper mapper, ISearchApiClient searchApiClient, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _mapper = mapper;
            _searchApiClient = searchApiClient;
            _contextAccessor = contextAccessor;
        }

        public async Task<SearchRequestViewModel> Search(SearchRequestViewModel vm)
        {
            var epaOrgId = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            var username = _contextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var results = await _searchApiClient.SearchLearners(new LearnerSearchRequest()
            {
                Surname = vm.Surname,
                Uln = long.Parse(vm.Uln),
                EpaOrgId = epaOrgId,
                Username = username
            });

            var viewModelSearchResults = new List<ResultViewModel>();
            foreach (var result in results)
            {
                viewModelSearchResults.Add(new ResultViewModel
                {
                    GivenNames = result.GivenNames,
                    FamilyName = result.FamilyName,
                    Uln = Convert.ToString(result.Uln),
                    Standard = result.Standard,
                    StdCode = Convert.ToString(result.StdCode),
                    Version = result.Version,
                    VersionConfirmed = result.VersionConfirmed,
                    Versions = _mapper.Map<List<StandardVersionViewModel>>(result.Versions),
                    OverallGrade = result.OverallGrade,
                    CertificateReference = result.CertificateReference,
                    CertificateStatus = result.CertificateStatus,
                    IsPrivatelyFunded = result.IsPrivatelyFunded,
                    Level = Convert.ToString(result.Level),
                    SubmittedAt = result.SubmittedAt,
                    SubmittedBy = result.SubmittedBy,
                    AchDate = result.AchDate,
                    LearnStartDate = result.LearnStartDate,
                    ShowExtraInfo = result.ShowExtraInfo,
                    UlnAlreadyExists = result.UlnAlreadyExits,
                    IsNoMatchingFamilyName = result.IsNoMatchingFamilyName
                });
            }

            vm.SearchResults = viewModelSearchResults;
            return vm;
        }
    }
}