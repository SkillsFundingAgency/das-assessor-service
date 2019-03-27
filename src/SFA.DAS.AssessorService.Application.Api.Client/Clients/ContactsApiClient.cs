using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ContactsApiClient : ApiClientBase, IContactsApiClient
    {
        private readonly ILogger<ContactsApiClient> _logger;
        private readonly IContactApplyClient _contactApplyClient;

        public ContactsApiClient(string baseUri, ITokenService tokenService, ILogger<ContactsApiClient> logger, IContactApplyClient contactApplyClient) : base(baseUri, tokenService, logger)
        {
            _logger = logger;
            _contactApplyClient = contactApplyClient;
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

        public async Task<ContactResponse> GetById(string id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/user/{id}"))
            {
                return await RequestAndDeserialiseAsync<ContactResponse>(request, $"Could not find the contact");
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

        public async Task<ContactResponse> GetContactBySignInId(string signInId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/signInId/{signInId}"))
            {
                return await RequestAndDeserialiseAsync<ContactResponse>(request, $"Could not find contact with {signInId}");
            }
        }

        public async Task<List<ContactResponse>> GetAllContactsForOrganisation(string epaoId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/get-all/{epaoId}"))
            {
                return await RequestAndDeserialiseAsync<List<ContactResponse>>(request, $"Could not find contact with organisation {epaoId}");
            }
        }

        public async Task<ContactResponse> UpdateOrgAndStatus(UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStausRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/updateContactWithOrgAndStatus"))
            {
                return await PostPutRequestWithResponse<UpdateContactWithOrgAndStausRequest, ContactResponse>(request,updateContactWithOrgAndStausRequest);
            }
        }

        public async Task<ContactBoolResponse> InviteUser(CreateContactRequest createContactRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts"))
            {
                var response =
                    await PostPutRequestWithResponse<CreateContactRequest, ContactBoolResponse>(request,
                        createContactRequest);
                if (response.Result)
                {
                    await _contactApplyClient.CreateAccountInApply(new NewApplyContact
                    {
                        Email = createContactRequest.Email, GivenName = createContactRequest.GivenName,
                        FamilyName = createContactRequest.FamilyName,
                        FromAssessor = true
                    });
                }

                return response;
            }
        }

        public async Task Callback(DfeSignInCallback callback)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts/callback"))
            {
                await PostPutRequest(request, callback);
                var response = await GetContactBySignInId(callback.Sub);
                await _contactApplyClient.UpdateApplySignInId(new AddToApplyContactASignInId
                {
                    Email = response.Email,
                    SignInId = callback.Sub,
                    ContactId = callback.SourceId,
                    UpdatedBy = "AssessorSignIn"
                });
            }
        }

        public async Task MigrateUsers()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/MigrateUsers"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());
                request.Headers.Add("Accept", "application/json");
                request.Content = new StringContent("", System.Text.Encoding.UTF8, "application/json");

                await HttpClient.SendAsync(request);
            }
        }
    }

    public interface IContactsApiClient
    {
        Task<ContactResponse> GetByUsername(string username);
        
        Task<ContactResponse> Update(UpdateContactRequest updateContactRequest);

        Task<List<ContactsWithPrivilegesResponse>> GetContactsWithPrivileges(string endPointAssessorOrganisationId);

        Task<ContactResponse> UpdateStatus(UpdateContactStatusRequest updateContactStatusRequest);

        Task<ContactResponse> GetById(string id);

        Task<ContactResponse> GetContactBySignInId(string signInId);
        Task<List<ContactResponse>> GetAllContactsForOrganisation(string epaoId);

        Task<ContactResponse> UpdateOrgAndStatus(
            UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStausRequest);

        Task<ContactBoolResponse> InviteUser(CreateContactRequest createAccountRequest);
        Task Callback(DfeSignInCallback callback);

        Task MigrateUsers();
    }
}