using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.SubmissionEvents.Types;

namespace SFA.DAS.AssessorService.ExternalApis.SubmissionEvents
{
    public class SubmissionEventProviderApiClient : ApiClientBase, ISubmissionEventProviderApiClient
    {
        public SubmissionEventProviderApiClient(string baseUri, string bearerToken, string version) : base(baseUri,bearerToken,version)
        {
        }

        public SubmissionEventProviderApiClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<SubmissionEvent>> GetLatestLearnerEventForStandards(long uln, long sinceEventId = 0)
        {
            var url = $"/api/learners?uln={uln}";
            if (sinceEventId > 0)
            {
                url += $"&sinceEventId={sinceEventId}";
            }

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                return await RequestAndDeserialiseAsync<List<SubmissionEvent>>(request, $"Could not find submissions since the event {sinceEventId}");
            }
        }
        
    }
}
