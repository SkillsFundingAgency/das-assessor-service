using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class UkrlpApiClient : ApiClientBase, IUkrlpApiClient
    {
        public UkrlpApiClient(string baseUri, ITokenService tokenService,
            ILogger<UkrlpApiClient> logger) : base(baseUri, tokenService, logger)
        {
        }

        public UkrlpApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }


        public async Task<UkrlpProviderDetails> Get(string ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/ukrlp/provider-details/{WebUtility.UrlEncode(ukprn)}"))
            {
                return await RequestAndDeserialiseAsync<UkrlpProviderDetails>(request,
                    $"Could not find the provider details from the ukprn {ukprn}");
            }

            //using (var request = new HttpRequestMessage(HttpMethod.Get, $"{WebUtility.UrlEncode(ukprn)}"))
            //{
            //    return await RequestAndDeserialiseAsync<UkrlpProviderDetails>(request,
            //        $"Could not find the provider details from the ukprn {ukprn}");
            //}
        }
    }
}
