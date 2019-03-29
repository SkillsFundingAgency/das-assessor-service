using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.ApplyTypes;
using CreateOrganisationRequest = SFA.DAS.AssessorService.ApplyTypes.CreateOrganisationRequest;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class OrganisationsApplyApiClient:ApiClientBase, IOrganisationsApplyApiClient
    {
        public OrganisationsApplyApiClient(string baseUri, ITokenService applyTokenService,
            ILogger<OrganisationsApiClient> logger) : base(baseUri, applyTokenService, logger)
        {
        }
        
        public OrganisationsApplyApiClient(HttpClient httpClient, ITokenService applyTokenService, ILogger<ApiClientBase> logger) : base(httpClient, applyTokenService, logger)
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

        public async Task<Organisation> CreateNewOrganisation(CreateOrganisationRequest createOrganisationRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/organisations"))
            {
                return await PostPutRequestWithResponse<CreateOrganisationRequest, Organisation>(request, createOrganisationRequest);
            }
        }

        public async Task<Organisation> DoesOrganisationExist(string name)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/organisations/name/{name}"))
            {
                return await RequestAndDeserialiseAsync<Organisation>(request);
            }
        }
    }


    public interface IOrganisationsApplyApiClient
    {
        Task<IEnumerable<OrganisationSearchResult>> SearchForOrganisations(string searchTerm);
        Task<Organisation> ConfirmSearchedOrganisation(CreateOrganisationRequest createOrganisationRequest);
        Task<Organisation> CreateNewOrganisation(CreateOrganisationRequest createOrganisationRequest);
        Task<Organisation> DoesOrganisationExist(string name);
    }
}
