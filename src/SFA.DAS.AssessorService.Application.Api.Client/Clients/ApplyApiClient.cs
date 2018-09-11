using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ApplyApiClient : ApiClientBase, IApplyApiClient
    {
        public ApplyApiClient(string baseUri, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(baseUri, tokenService, logger)
        {
        }


        public async Task<List<SequenceSummary>> GetSequenceSummary(string userId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/questions/summary/{userId}"))
            {
                return await RequestAndDeserialiseAsync<List<SequenceSummary>>(httpRequest, "Could not get Sequence Summary");
            }
        }

        public async Task<Sequence> GetSequence(string userId, string sequenceId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/questions/sequence/{userId}/{sequenceId}"))
            {
                return await RequestAndDeserialiseAsync<Sequence>(httpRequest, "Could not get Sequence");
            }
        }

        public async Task<Page> GetPage(string userId, string pageId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/questions/page/{userId}/{pageId}"))
            {
                return await RequestAndDeserialiseAsync<Page>(httpRequest, "Could not get Page");
            }
        }
    }

    public interface IApplyApiClient
    {
        Task<List<SequenceSummary>> GetSequenceSummary(string userId);
        Task<Sequence> GetSequence(string userId, string sequenceId);
        Task<Page> GetPage(string userId, string pageId);
    }
}