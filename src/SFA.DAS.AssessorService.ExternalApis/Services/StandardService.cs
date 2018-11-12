using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public StandardService(CacheService cacheService, IAssessmentOrgsApiClient assessmentOrgsApiClient, IIfaStandardsApiClient ifaStandardsApiClient)
        {
            _cacheService = cacheService;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
            _ifaStandardsApiClient = ifaStandardsApiClient;
        }

        public async Task<IEnumerable<StandardSummary>> GetAllStandardSummaries()
        {          
            var results = await _cacheService.RetrieveFromCache<IEnumerable<StandardSummary>>("StandardSummaries");
            if (results != null) return results;

            var standardSummaries = await _assessmentOrgsApiClient.GetAllStandardSummaries();
            await _cacheService.SaveToCache("StandardSummaries", standardSummaries, 8);
            return standardSummaries;

        }

        public async Task<IEnumerable<StandardCollation>> GatherAllStandardDetails()
        {
            var hoursBetweenCaching = 8;
            var ifaResults = await _cacheService.RetrieveFromCache<IEnumerable<IfaStandard>>("IfaStandardSummaries");

            if (ifaResults == null)
            {
                var ifaStandards = await _ifaStandardsApiClient.GetAllStandards();
                var fullIfaStandards = new List<IfaStandard>();
                foreach (var ifaStandard in ifaStandards)
                {
                    fullIfaStandards.Add(await _ifaStandardsApiClient.GetStandard(ifaStandard.Id));
                }

                ifaResults = fullIfaStandards;
                await _cacheService.SaveToCache("IfaStandardSummaries", fullIfaStandards, hoursBetweenCaching);
            }

            var winResults = await _cacheService.RetrieveFromCache<IEnumerable<StandardSummary>>("StandardSummaries");
            if (winResults == null)
            { 
            var standardSummaries = await _assessmentOrgsApiClient.GetAllStandardSummaries();
            await _cacheService.SaveToCache("StandardSummaries", standardSummaries, hoursBetweenCaching);
                winResults = standardSummaries;

            }

            var collation = new List<StandardCollation>();
            foreach (var winStandard in winResults)
            {
                var ifaStandardToMatch = ifaResults.FirstOrDefault(x => x.Id.ToString() == winStandard.Id);
                if (!int.TryParse(winStandard.Id, out int standardId)) continue;
                var standard = new StandardCollation
                {
                    StandardId = standardId,
                    ReferenceNumber = ifaStandardToMatch?.ReferenceNumber,
                    Title = winStandard.Title,
                    StandardData = new StandardData
                    {
                        Category = ifaStandardToMatch?.Category,
                        IfaStatus = ifaStandardToMatch?.Status,
                        EffectiveFrom = winStandard.EffectiveFrom,
                        EffectiveTo = winStandard.EffectiveTo,
                        Level = winStandard.Level,
                        LastDateForNewStarts = winStandard.LastDateForNewStarts
                    }
                };
                collation.Add(standard);
            }

            return collation;
        }


        public async Task<Standard> GetStandard(int standardId)
        {
            return await _assessmentOrgsApiClient.GetStandard(standardId);
        }
    }

    public interface IStandardService
    {
        Task<IEnumerable<StandardSummary>> GetAllStandardSummaries();

        Task<IEnumerable<StandardCollation>> GatherAllStandardDetails();

        Task<Standard> GetStandard(int standardId);
    }
}
