using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;


namespace SFA.DAS.AssessorService.Web.Staff.Services
{
    public class StandardService: IStandardService
    {
        private readonly CacheHelper _cacheHelper;
        private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;

        public StandardService(CacheHelper cacheHelper, IAssessmentOrgsApiClient assessmentOrgsApiClient)
        {
            _cacheHelper = cacheHelper;
            _assessmentOrgsApiClient = assessmentOrgsApiClient;
        }

        public async Task<IEnumerable<StandardSummary>> GetAllStandardSummaries()
        {
            var results = await _cacheHelper.RetrieveFromCache<IEnumerable<StandardSummary>>("StandardSummaries");
           if (results != null) return results;
            var standardSummaries = await _assessmentOrgsApiClient.GetAllStandardSummaries();
            await _cacheHelper.SaveToCache("StandardSummaries", standardSummaries, 8);  
            return standardSummaries;

        }

        public async Task<Standard> GetStandard(int standardId)
        {
            return await _assessmentOrgsApiClient.GetStandard(standardId);
        }
    }

    public interface IStandardService
    {
        Task<IEnumerable<StandardSummary>> GetAllStandardSummaries();
        Task<Standard> GetStandard(int standardId);
    }
}
