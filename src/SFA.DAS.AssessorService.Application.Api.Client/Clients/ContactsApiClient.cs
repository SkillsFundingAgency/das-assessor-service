using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ContactsApiClient : ApiClientBase, IContactsApiClient
    {
        private readonly ILogger<ContactsApiClient> _logger;

        public ContactsApiClient(string baseUri, ITokenService tokenService, ILogger<ContactsApiClient> logger) : base(baseUri, tokenService, logger)
        {
            _logger = logger;
        }

        public ContactsApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }

        public async Task<ContactResponse> GetByUsername(string username)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/user/{username}"))
            {
                return await RequestAndDeserialiseAsync<ContactResponse>(request, $"Could not find the contact");
            }
        }

        public async Task<ContactResponse> Create(CreateContactRequest contact)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts/"))
            {
                return await PostPutRequestWithResponse<CreateContactRequest, ContactResponse>(request, contact);
            }
        }

        public async Task<ContactResponse> Update(UpdateContactRequest updateContactRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/"))
            {
                return await PostPutRequestWithResponse<UpdateContactRequest, ContactResponse>(request, updateContactRequest);
            }
        }

        public async Task<ContactResponse> UpdateStatus(UpdateContactStatusRequest updateContactStatusRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/status"))
            {
                return await PostPutRequestWithResponse<UpdateContactStatusRequest, ContactResponse>(request, updateContactStatusRequest);
            }
        }

        public async Task<List<ContactsWithPrivilegesResponse>> GetContactsWithPrivileges(string endPointAssessorOrganisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/{endPointAssessorOrganisationId}/withprivileges"))
            {
                return await RequestAndDeserialiseAsync<List<ContactsWithPrivilegesResponse>>(request, $"Could not find contacts for {endPointAssessorOrganisationId}");
            }
        }
        
    }

    public interface IContactsApiClient
    {
        Task<ContactResponse> GetByUsername(string username);

        Task<ContactResponse> Create(CreateContactRequest contact);

        Task<ContactResponse> Update(UpdateContactRequest updateContactRequest);

        Task<List<ContactsWithPrivilegesResponse>> GetContactsWithPrivileges(string endPointAssessorOrganisationId);

        Task<ContactResponse> UpdateStatus(UpdateContactStatusRequest updateContactStatusRequest);
        
    }
}