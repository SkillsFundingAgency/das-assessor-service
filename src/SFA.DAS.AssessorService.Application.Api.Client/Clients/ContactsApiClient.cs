using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ContactsApiClient : ApiClientBase, IContactsApiClient
    {
        public ContactsApiClient(string baseUri, ITokenService tokenService) : base(baseUri, tokenService)
        {
        }

        public async Task<Contact> GetByUsername(string userKey, string username)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/user/{username}"))
            {
                return await RequestAndDeserialiseAsync<Contact>(userKey, request, $"Could not find the contact");
            }
        }

        public async Task<Contact> Create(string userKey, CreateContactRequest contact)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts/"))
            {
                return await PostPutRequestWithResponse<CreateContactRequest, Contact>(userKey, request, contact);
            }
        }

        //public async Task<Organisation> Get(string userKey, string ukprn)
        //{
        //    using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/{ukprn}"))
        //    {
        //        return await RequestAndDeserialiseAsync<Organisation>(userKey, request, $"Could not find the organisation {ukprn}");
        //    }
        //}

        //public async Task Create(string userKey, CreateOrganisationRequest organisationCreateViewModel)
        //{
        //    using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts/"))
        //    {
        //        await PostPutRequest(userKey, request, organisationCreateViewModel);
        //    }
        //}

        //public async Task Update(string userKey, UpdateOrganisationRequest organisationUpdateViewModel)
        //{
        //    using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/"))
        //    {
        //        await PostPutRequest(userKey, request, organisationUpdateViewModel);
        //    }
        //}

        //public async Task Delete(string userKey, Guid id)
        //{
        //    using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/contacts/"))
        //    {
        //        await Delete(userKey, request);
        //    }
        //}
    }

    public interface IContactsApiClient
    {
        Task<Contact> GetByUsername(string userKey, string username);

        //Task<IEnumerable<Organisation>> GetAll(string userKey);
        //Task<Organisation> Get(string userKey, string ukprn);
        //Task Create(string userKey, CreateOrganisationRequest organisationCreateViewModel);
        //Task Update(string userKey, UpdateOrganisationRequest organisationUpdateViewModel);
        //Task Delete(string userKey, Guid id);
        Task<Contact> Create(string userKey, CreateContactRequest contact);
    }
}