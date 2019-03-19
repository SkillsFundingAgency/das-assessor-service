using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
   
    public class ContactApplyClient : ApiClientBase, IContactApplyClient
    {
        public ContactApplyClient(string baseUri, ITokenService applyTokenService,
            ILogger<ContactApplyClient> logger) : base(baseUri, applyTokenService, logger)
        {
        }

        public ContactApplyClient(HttpClient httpClient, ITokenService applyTokenService, ILogger<ApiClientBase> logger) : base(httpClient, applyTokenService, logger)
        {
        }

        public async Task CreateAccountInApply(NewApplyContact contact)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post,
                $"/Account"))
            {
                 await PostPutRequest(request,contact);
            }

        }

        public async Task UpdateApplySignInId(AddToApplyContactASignInId addToApplyContactASignInId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put,
                $"/Account"))
            {
                await PostPutRequest(request, addToApplyContactASignInId);
            }
        }
    }
}


public interface IContactApplyClient
{
    Task CreateAccountInApply(NewApplyContact contact);
    Task UpdateApplySignInId(AddToApplyContactASignInId addToApplyContactASignInId);
}