using System;
using System.Collections.Generic;
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


        public async Task<IfaStandard> GetStandard(int standardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/standards/{standardId}"))
            {
                return await RequestAndDeserialiseAsync<IfaStandard>(request);
            }
        }

        public async Task<List<IfaStandard>> GetAllStandards()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/standards"))
            {
                return await RequestAndDeserialiseAsync<List<IfaStandard>>(request);
            }
        }
    }
}
