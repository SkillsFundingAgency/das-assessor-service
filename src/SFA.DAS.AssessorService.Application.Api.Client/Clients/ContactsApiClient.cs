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

        public async Task<Contact> Update(string userKey, UpdateContactRequest updateContactRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/"))
            {
                return await PostPutRequestWithResponse<UpdateContactRequest, Contact>(userKey, request, updateContactRequest);
            }
        }
    }

    public interface IContactsApiClient
    {
        Task<Contact> GetByUsername(string userKey, string username);

        Task<Contact> Create(string userKey, CreateContactRequest contact);

        Task<Contact> Update(string userKey, UpdateContactRequest updateContactRequest);
    }
}