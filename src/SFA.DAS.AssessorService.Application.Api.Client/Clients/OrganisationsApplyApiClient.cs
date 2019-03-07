using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.ApplyTypes;
using CreateOrganisationRequest = SFA.DAS.AssessorService.ApplyTypes.CreateOrganisationRequest;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class OrganisationsApplyApiClient:ApiClientBase, IOrganisationsApplyApiClient
    {
        public OrganisationsApplyApiClient(string baseUri, IEnumerable<ITokenService> tokenService,
            ILogger<OrganisationsApiClient> logger) : base(baseUri, tokenService, logger, "ApplyTokenService")
        {
        }
        
        public OrganisationsApplyApiClient(HttpClient httpClient, IEnumerable<ITokenService> tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }

        public async Task<IEnumerable<OrganisationSearchResult>> SearchForOrganisations(string searchTerm)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get,
                $"/OrganisationSearch?searchTerm={searchTerm}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<OrganisationSearchResult>>(request,
                    $"Could not retrieve organisations for search {searchTerm}.");
            }
            
        }

        public async Task<Organisation> ConfirmSearchedOrganisation(CreateOrganisationRequest createOrganisationRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post,
                $"/organisations"))
            {
               return await PostPutRequestWithResponse<CreateOrganisationRequest,Organisation>(request, createOrganisationRequest);
            }
        }
    }


    public interface IOrganisationsApplyApiClient
    {
        Task<IEnumerable<OrganisationSearchResult>> SearchForOrganisations(string searchTerm);
        Task<Organisation> ConfirmSearchedOrganisation(CreateOrganisationRequest createOrganisationRequest);
    }
}
