using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Domain.Entities;

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

        public async Task<List<Privilege>> GetPrivileges()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/privileges"))
            {
                return await RequestAndDeserialiseAsync<List<Privilege>>(request, $"Could not privileges");
            }
        }
        
        public async Task<List<ContactsPrivilege>> GetContactPrivileges(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/user/{userId}/privileges"))
            {
                return await RequestAndDeserialiseAsync<List<ContactsPrivilege>>(request, $"Could not find the contact");
            }
        }

        public async Task<ContactResponse> GetByUsername(string username)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/user/{WebUtility.UrlEncode(username)}"))
            {
                return await RequestAndDeserialiseAsync<ContactResponse>(request, $"Could not find the contact");
            }
        }

        public async Task<ContactResponse> GetById(Guid id)
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

        public async Task<List<ContactsWithPrivilegesResponse>> GetContactsWithPrivileges(Guid organisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/{organisationId}/withprivileges"))
            {
                return await RequestAndDeserialiseAsync<List<ContactsWithPrivilegesResponse>>(request, $"Could not find contacts for {organisationId}");
            }
        }

        public async Task<ContactBoolResponse> DoesContactHavePrivileges(string userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/user/{userId}/haveprivileges"))
            {
                return await RequestAndDeserialiseAsync<ContactBoolResponse>(request, $"Could not find contact with {userId}");
            }
        }

        public async Task<ContactResponse> GetContactBySignInId(string signInId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/signInId/{signInId}"))
            {
                var result = await RequestAndDeserialiseAsync<ContactResponse>(request, $"Could not find contact with {signInId}");
                return result;
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
                return await PostPutRequestWithResponse<UpdateContactWithOrgAndStausRequest, ContactResponse>(request, updateContactWithOrgAndStausRequest);
            }
        }

        public async Task<ContactBoolResponse> InviteUser(CreateContactRequest createContactRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts"))
            {
                var response =
                    await PostPutRequestWithResponse<CreateContactRequest, ContactBoolResponse>(request,
                        createContactRequest);

                return response;
            }
        }

        public async Task Callback(DfeSignInCallback callback)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts/callback"))
            {
                await PostPutRequest(request, callback);
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

        public async Task MigrateContactsAndOrgsToApply()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/MigrateContactsAndOrgsToApply"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());
                request.Headers.Add("Accept", "application/json");
                request.Content = new StringContent("", System.Text.Encoding.UTF8, "application/json");

                await HttpClient.SendAsync(request);
            }
        }

        public async Task MigrateSingleContactToApply(System.Guid signinId)
        {
            var signinIdWrapper = new SigninIdWrapper(signinId);
            _logger.LogInformation($"MigrateSingleContactToApply json being POSTed: {JsonConvert.SerializeObject(signinIdWrapper)}");
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/MigrateSingleContactToApply"))
            {
                await PostPutRequest(request, signinIdWrapper);
            }
        }

        public async Task<ContactResponse> CreateANewContactWithGivenId(Contact contact)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/createNewContactWithGivenId"))
            {
                var response =
                     await PostPutRequestWithResponse<Contact,ContactResponse>(request,contact);

                return response;
            }
        }

        public async Task AssociateDefaultRolesAndPrivileges(Contact contact)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/associateDefaultRolesAndPrivileges"))
            {
                 await PostPutRequest(request, contact);
                
            }
        }

        public async Task<SetContactPrivilegesResponse> SetContactPrivileges(SetContactPrivilegesRequest privilegesRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/setContactPrivileges"))
            {
                return await PostPutRequestWithResponse<SetContactPrivilegesRequest,SetContactPrivilegesResponse>(request, privilegesRequest);
            }
        }

        public async Task<RemoveContactFromOrganisationResponse> RemoveContactFromOrganisation(Guid requestingUserId, Guid contactId)
        {
            var removeContactRequest = new RemoveContactFromOrganisationRequest(requestingUserId, contactId);
            
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/removeContactFromOrganisation"))
            {
                return await PostPutRequestWithResponse<RemoveContactFromOrganisationRequest,RemoveContactFromOrganisationResponse>(request, removeContactRequest);
            }
        }

        public async Task<InviteContactToOrganisationResponse> InviteContactToOrganisation(InviteContactToOrganisationRequest invitationRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/inviteContactToOrganisation"))
            {
                return await PostPutRequestWithResponse<InviteContactToOrganisationRequest,InviteContactToOrganisationResponse>(request, invitationRequest);
            }
        }
    }
}