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

        public async Task<OrganisationQueryViewModel> Get(int ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/assessment-providers/{ukprn}"))
            {
                return await RequestAndDeserialiseAsync<OrganisationQueryViewModel>(request, $"Could not find the organisation {ukprn}");
            }
        }

        public async Task Create(int ukprn, OrganisationCreateViewModel organisationCreateViewModel)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/assessment-providers/{ukprn}"))
            {
                await PostPutRequest(request, organisationCreateViewModel);
            }
        }

        public async Task Update(int ukprn, OrganisationUpdateViewModel organisationUpdateViewModel)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/assessment-providers/{ukprn}"))
            {
                await PostPutRequest(request, organisationUpdateViewModel);
            }
        }
    }

    public interface IOrganisationsApiClient
    {
        Task<OrganisationQueryViewModel> Get(int ukprn);
        Task Create(int ukprn, OrganisationCreateViewModel organisationCreateViewModel);
        Task Update(int ukprn, OrganisationUpdateViewModel organisationUpdateViewModel);
    }
}