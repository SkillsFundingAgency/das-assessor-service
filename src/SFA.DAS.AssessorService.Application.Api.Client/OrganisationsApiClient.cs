using System;
using System.Collections.Generic;
using SFA.DAS.AssessorService.ViewModel.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class OrganisationsApiClient : ApiClientBase, IOrganisationsApiClient
    {
        public OrganisationsApiClient(string baseUri, ITokenService tokenService) : base(baseUri, tokenService)
        {
        }

        public async Task<IEnumerable<OrganisationQueryViewModel>> GetAll(string userKey)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/assessment-providers/"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<OrganisationQueryViewModel>>(userKey, request, $"Could not find the organisations");
            }
        }

        public async Task<OrganisationQueryViewModel> Get(string userKey, string ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/assessment-providers/{ukprn}"))
            {
                return await RequestAndDeserialiseAsync<OrganisationQueryViewModel>(userKey, request, $"Could not find the organisation {ukprn}");
            }
        }

        public async Task Create(string userKey, OrganisationCreateViewModel organisationCreateViewModel)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/assessment-providers/"))
            {
                await PostPutRequest(userKey, request, organisationCreateViewModel);
            }
        }

        public async Task Update(string userKey, OrganisationUpdateViewModel organisationUpdateViewModel)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/assessment-providers/"))
            {
                await PostPutRequest(userKey, request, organisationUpdateViewModel);
            }
        }

        public async Task Delete(string userKey, Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/assessment-providers/"))
            {
                await Delete(userKey, request);
            }
        }
    }

    public interface IOrganisationsApiClient
    {
        Task<IEnumerable<OrganisationQueryViewModel>> GetAll(string userKey);
        Task<OrganisationQueryViewModel> Get(string userKey, string ukprn);
        Task Create(string userKey, OrganisationCreateViewModel organisationCreateViewModel);
        Task Update(string userKey, OrganisationUpdateViewModel organisationUpdateViewModel);
        Task Delete(string userKey, Guid id);
    }
}