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

        public async Task<Contact> GetByUsername(string username)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/user/{username}"))
            {
                return await RequestAndDeserialiseAsync<Contact>(request, $"Could not find the contact");
            }
        }

        public async Task<Contact> Create(CreateContactRequest contact)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts/"))
            {
                return await PostPutRequestWithResponse<CreateContactRequest, Contact>(request, contact);
            }
        }

        public async Task<Contact> Update(UpdateContactRequest updateContactRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/"))
            {
                return await PostPutRequestWithResponse<UpdateContactRequest, Contact>(request, updateContactRequest);
            }
        }
    }

    public interface IContactsApiClient
    {
        Task<Contact> GetByUsername(string username);

        Task<Contact> Create(CreateContactRequest contact);

        Task<Contact> Update(UpdateContactRequest updateContactRequest);
    }
}