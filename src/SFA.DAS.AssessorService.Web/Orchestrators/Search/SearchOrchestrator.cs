using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.ViewModels.Search;
using static SFA.DAS.AssessorService.Web.ViewModels.Search.ResultViewModel;

namespace SFA.DAS.AssessorService.Web.Orchestrators.Search
{
    public class SearchOrchestrator : ISearchOrchestrator
    {
        private readonly ILogger<SearchController> _logger;
        private readonly ISearchApiClient _searchApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public SearchOrchestrator(ILogger<SearchController> logger, ISearchApiClient searchApiClient, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _searchApiClient = searchApiClient;
            _contextAccessor = contextAccessor;
        }

        public async Task<SearchRequestViewModel> Search(SearchRequestViewModel vm)
        {
            var epaOrgId = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            var username = _contextAccessor.HttpContext.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;

            var results = await _searchApiClient.Search(new SearchQuery()
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
                    Versions = result.Versions.Select(s => new StandardVersion { StandardUId = s.StandardUId, Title = s.Title, Version = s.Version }).ToList(),
                    OverallGrade = result.OverallGrade,
                    CertificateReference = result.CertificateReference,
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