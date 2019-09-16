using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;

namespace SFA.DAS.AssessorService.ExternalApis.IFAStandards
{
    public class IfaStandardsApiClient : ApiClientBase, IIfaStandardsApiClient
    {
        public IfaStandardsApiClient(string baseUri = null) : base(baseUri)
        {
        }

        public IfaStandardsApiClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<IfaStandard> GetStandard(int standardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/apprenticeshipstandards/{standardId}"))
            {
                return await RequestAndDeserialiseAsync<IfaStandard>(request);
            }
        }

        public async Task<List<IfaStandard>> GetAllStandards()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/apprenticeshipstandards"))
            {
                // the list of standards returned from the api can contain null entries and entries
                // with a LarsCode = 0; the ones with a LarsCode = 0 are not approved standards that are imported
                // into a separate table from the LarsCode != 0 imported into [StandardCollation]
                var allStandards = await RequestAndDeserialiseAsync<List<IfaStandard>>(request);
                return allStandards?.FindAll(p => p != null);
            }
        }
    }
}
