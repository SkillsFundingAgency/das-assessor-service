using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Learner;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ApprovalsLearnerApiClient : ApiClientBase, IApprovalsLearnerApiClient
    {
        public ApprovalsLearnerApiClient(string baseUri, ITokenService tokenService, ILogger<ApprovalsLearnerApiClient> logger) 
            : base(baseUri, tokenService, logger)
        {
        }

        public ApprovalsLearnerApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) 
            : base(httpClient, tokenService, logger)
        {
        }

        public async Task<ApprovalsLearnerResult> GetLearnerRecord(int stdCode, long uln)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/approvals/learner?stdCode={stdCode}&uln={uln}"))
            {
                return await RequestAndDeserialiseAsync<ApprovalsLearnerResult>(request,
                    $"Could not find the learner record");
            }
        }
    }
}
