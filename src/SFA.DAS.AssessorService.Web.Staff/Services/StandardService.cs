using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;
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

        public async Task<IEnumerable<Standard>> GetAllStandards()
        {
            //var results = await _cacheHelper.RetrieveFromCache<IEnumerable<Standard>>("Standards");
            //if (results != null) return results;
            var standards = await _assessmentOrgsApiClient.GetAllStandards();
            //await _cacheHelper.SaveToCache("Standards", standards, 8);  // MFCMFC 8 hours???

            var results = standards;

            return results;

            // var results = System.Web.Caching.Cache["Standards"] as Standard[];


        }

        public async Task<Standard> GetStandard(int standardId)
        {
            return await _assessmentOrgsApiClient.GetStandard(standardId);
        }
    }

    public interface IStandardService
    {
        Task<IEnumerable<Standard>> GetAllStandards();
        Task<Standard> GetStandard(int standardId);
    }
}
