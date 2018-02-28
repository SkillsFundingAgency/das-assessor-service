using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    using AssessorService.Api.Types.Models;

    public class OrganisationsApiClient : ApiClientBase, IOrganisationsApiClient
    {
        public OrganisationsApiClient(string baseUri, ITokenService tokenService) : base(baseUri, tokenService)
        {
        }

        public async Task<IEnumerable<Organisation>> GetAll(string userKey)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<Organisation>>(userKey, request, $"Could not find the organisations");
            }
        }

        public async Task<Organisation> Get(string userKey, string ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/{ukprn}"))
            {
                return await RequestAndDeserialiseAsync<Organisation>(userKey, request, $"Could not find the organisation {ukprn}");
            }
        }

        public async Task Create(string userKey, CreateOrganisationRequest organisationCreateViewModel)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/organisations/"))
            {
                await PostPutRequest(userKey, request, organisationCreateViewModel);
            }
        }

        public async Task Update(string userKey, UpdateOrganisationRequest organisationUpdateViewModel)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/organisations/"))
            {
                await PostPutRequest(userKey, request, organisationUpdateViewModel);
            }
        }

        public async Task Delete(string userKey, Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/organisations/"))
            {
                await Delete(userKey, request);
            }
        }
    }

    public interface IOrganisationsApiClient
    {
        Task<IEnumerable<Organisation>> GetAll(string userKey);
        Task<Organisation> Get(string userKey, string ukprn);
        Task Create(string userKey, CreateOrganisationRequest organisationCreateViewModel);
        Task Update(string userKey, UpdateOrganisationRequest organisationUpdateViewModel);
        Task Delete(string userKey, Guid id);
    }
}