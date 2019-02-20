using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.StandardCollationApiClient.Types;

namespace SFA.DAS.AssessorService.ExternalApis.StandardCollationApiClient
{
    public class StandardCollationApiClient : ApiClientBase, IStandardCollationApiClient
    {
        public StandardCollationApiClient(string baseUri = null) : base(baseUri)
        {
        }

        public StandardCollationApiClient(HttpClient httpClient) : base(httpClient)
        {
        }
        public async Task<List<StandardCollation>> GetStandardCollations()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/ao/assessment-organisations/collated-standards"))
            {
                return await RequestAndDeserialiseAsync<List<StandardCollation>>(request);
            }
        }

        public async Task<StandardCollation> GetStandardCollation(int standardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/ao/assessment-organisations/collated-standards/{standardId}"))
            {
                return await RequestAndDeserialiseAsync<StandardCollation>(request);
            }
        }
    }

}

