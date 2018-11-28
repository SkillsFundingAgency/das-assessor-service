using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;
using SFA.DAS.AssessorService.ExternalApis.Services;


namespace SFA.DAS.AssessorService.Web.Staff.Services
{
    public class StandardService: IStandardService
    {
        private readonly CacheService _cacheService;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;
        private readonly IIfaStandardsApiClient _ifaStandardsApiClient;
        private readonly ILogger<StandardService> _logger;
        public StandardService(CacheService cacheService, IAssessmentOrgsApiClient assessmentOrgsApiClient, IIfaStandardsApiClient ifaStandardsApiClient, ILogger<StandardService> logger)
        {
            _cacheService = cacheService;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _ifaStandardsApiClient = ifaStandardsApiClient;
            _logger = logger;
        }

        public async Task<IEnumerable<StandardSummary>> GetAllStandardSummaries()
        {          
            var results = await _cacheService.RetrieveFromCache<IEnumerable<StandardSummary>>("StandardSummaries");
            if (results != null) return results;

            var standardSummaries = await _assessmentOrgsApiClient.GetAllStandardsV2();
            await _cacheService.SaveToCache("StandardSummaries", standardSummaries, 8);
            return standardSummaries;
        }

        public async Task<Standard> GetStandard(int standardId)
        {
            return await _assessmentOrgsApiClient.GetStandard(standardId);
        }
    

        public async Task<IEnumerable<StandardCollation>> GatherAllStandardDetails()
        {
            _logger.LogInformation("STANDARD COLLATION: Starting gathering of all IFA Standard details");
            var ifaStandards = await _ifaStandardsApiClient.GetAllStandards();
            _logger.LogInformation($"STANDARD COLLATION: Starting gathering of individual IFA Standard details: [{ifaStandards.Count}]");
            var ifaResults = await GatherIfaStandardsOneAtATime(ifaStandards);
            _logger.LogInformation("STANDARD COLLATION: Starting gathering of all Win Standard details");
            var winResults = await _assessmentOrgsApiClient.GetAllStandardsV2();
            _logger.LogInformation("STANDARD COLLATION: Start collating IFA and WIN standards");
            var collation = CollateWinAndIfaStandardDetails(winResults, ifaResults);
            _logger.LogInformation($"STANDARD COLLATION: Add unmatched Ifa Standards to list");
            AddIfaOnlyStandardsToGatheredStandards(ifaResults, collation);
            _logger.LogInformation($"STANDARD COLLATION: collation finished");
            return collation;
        }

        private static void AddIfaOnlyStandardsToGatheredStandards(List<IfaStandard> ifaResults, List<StandardCollation> collation)
        {
            var uncollatedIfaStandards = ifaResults.Where(ifaStandard => collation.All(s => s.StandardId != ifaStandard.Id))
                .ToList();

            foreach (var ifaStandard in uncollatedIfaStandards)
            {
                var standard = MapDataToStandardCollation(ifaStandard.Id, ifaStandard, null);
                collation.Add(standard);
            }
        }

        private static List<StandardCollation> CollateWinAndIfaStandardDetails(List<StandardSummary> winResults, List<IfaStandard> ifaResults)
        {
            var collation = new List<StandardCollation>();
            foreach (var winStandard in winResults)
            {
                var ifaStandardToMatch = ifaResults.FirstOrDefault(x => x.Id.ToString() == winStandard.Id);
                if (!int.TryParse(winStandard.Id, out int standardId)) continue;
                var standard = MapDataToStandardCollation(standardId, ifaStandardToMatch, winStandard);
                collation.Add(standard);
            }

            return collation;
        }

        private static StandardCollation MapDataToStandardCollation(int standardId, IfaStandard ifaStandard, StandardSummary winStandard)
        {
            return new StandardCollation
            {
                StandardId = standardId,
                ReferenceNumber = ifaStandard?.ReferenceNumber,
                Title = ifaStandard?.Title ?? winStandard?.Title,
                StandardData = new StandardData
                {
                    Category = ifaStandard?.Category,
                    IfaStatus = ifaStandard?.Status,
                    EffectiveFrom = winStandard?.EffectiveFrom,
                    EffectiveTo = winStandard?.EffectiveTo,
                    Level = winStandard?.Level ?? ifaStandard?.Level,
                    LastDateForNewStarts = winStandard?.LastDateForNewStarts,
                    IfaOnly = winStandard == null,
                    Duration = winStandard?.Duration ?? ifaStandard?.Duration,
                    MaxFunding = winStandard?.CurrentFundingCap ?? ifaStandard?.MaxFunding,
                    PublishedDate = ifaStandard?.PublishedDate,
                    IsPublished = winStandard?.IsPublished ?? ifaStandard?.IsPublished,
                    Ssa1 = ifaStandard?.Ssa1,
                    Ssa2 = ifaStandard?.Ssa2,
                    OverviewOfRole = ifaStandard?.OverviewOfRole,
                    IsActiveStandardInWin = winStandard?.IsActiveStandard,
                    FatUri =  winStandard?.Uri,
                    IfaUri =  ifaStandard?.Uri
                }
            };
        }

        private async Task<List<IfaStandard>> GatherIfaStandardsOneAtATime(IEnumerable<IfaStandard> ifaStandards)
        {
            var fullIfaStandards = new List<IfaStandard>();
            foreach (var ifaStandard in ifaStandards)
            {
                fullIfaStandards.Add(await _ifaStandardsApiClient.GetStandard(ifaStandard.Id));
            }

            return fullIfaStandards;
        }     
    }

    public interface IStandardService
    {
        Task<IEnumerable<StandardSummary>> GetAllStandardSummaries();
        Task<IEnumerable<StandardCollation>> GatherAllStandardDetails();
        Task<Standard> GetStandard(int standardId);
    }
}
