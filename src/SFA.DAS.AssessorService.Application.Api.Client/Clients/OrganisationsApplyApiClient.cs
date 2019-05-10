﻿using System;
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
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get,
                    $"/OrganisationSearch?searchTerm={searchTerm}"))
                {
                    return await RequestAndDeserialiseAsync<IEnumerable<OrganisationSearchResult>>(request,
                        $"Could not retrieve organisations for search {searchTerm}.");
                }
            }catch(HttpRequestException err)
            {
                if (err.Message.Contains("204"))
                    return null;
                throw err;
            }
            
        }

        public async Task<bool> IsCompanyActivelyTrading(string companyNumber)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get,  $"/OrganisationSearch/{companyNumber}/isActivelyTrading"))
            {
                return await RequestAndDeserialiseAsync<bool>(request, $"Could not retrieve trading details for the organisation with an company number of {companyNumber}");
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

        public async Task<Organisation> GetOrganisationByUserId(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/organisations/userid/{userId}"))
            {
                return await RequestAndDeserialiseAsync<Organisation>(request);
            }
        }
    }


    public interface IOrganisationsApplyApiClient
    {
        Task<IEnumerable<OrganisationSearchResult>> SearchForOrganisations(string searchTerm);
        Task<bool> IsCompanyActivelyTrading(string companyNumber);
        Task<Organisation> ConfirmSearchedOrganisation(CreateOrganisationRequest createOrganisationRequest);
        Task<Organisation> CreateNewOrganisation(CreateOrganisationRequest createOrganisationRequest);
        Task<Organisation> DoesOrganisationExist(string name);
        Task<Organisation> GetOrganisationByUserId(Guid userId);
    }
}
