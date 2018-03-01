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

        public async Task<IEnumerable<Organisation>> GetAll()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<Organisation>>(request, $"Could not find the organisations");
            }
        }

        public async Task<Organisation> Get(string ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organisations/{ukprn}"))
            {
                return await RequestAndDeserialiseAsync<Organisation>(request, $"Could not find the organisation {ukprn}");
            }
        }

        public async Task Create(CreateOrganisationRequest organisationCreateViewModel)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/organisations/"))
            {
                await PostPutRequest(request, organisationCreateViewModel);
            }
        }

        public async Task Update(UpdateOrganisationRequest organisationUpdateViewModel)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/organisations/"))
            {
                await PostPutRequest(request, organisationUpdateViewModel);
            }
        }

        public async Task Delete(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/organisations/"))
            {
                await Delete(request);
            }
        }
    }

    public interface IOrganisationsApiClient
    {
        Task<IEnumerable<Organisation>> GetAll();
        Task<Organisation> Get(string ukprn);
        Task Create(CreateOrganisationRequest organisationCreateViewModel);
        Task Update(UpdateOrganisationRequest organisationUpdateViewModel);
        Task Delete(Guid id);
    }
}