using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ContactsApiClient : ApiClientBase, IContactsApiClient
    {
        private readonly ILogger<ContactsApiClient> _logger;

        public ContactsApiClient(IAssessorApiClientFactory clientFactory, ILogger<ContactsApiClient> logger)
            : base(clientFactory.CreateHttpClient(), logger)
        {
            _logger = logger;
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
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/username/{WebUtility.UrlEncode(username)}"))
            {
                return await RequestAndDeserialiseAsync<ContactResponse>(request, $"Could not find the contact");
            }
        }
        public async Task<ContactResponse> GetContactByGovIdentifier(string govIdentifier)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/govidentifier/{WebUtility.UrlEncode(govIdentifier)}"))
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
                return await PostPutRequestWithResponseAsync<UpdateContactRequest, ContactResponse>(request, updateContactRequest);
            }
        }
        
        public async Task<ContactResponse> UpdateFromGovLogin(UpdateContactGovLoginRequest updateContactRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/govlogin"))
            {
                return await PostPutRequestWithResponseAsync<UpdateContactGovLoginRequest, ContactResponse>(request, updateContactRequest);
            }
        }

        public async Task<ContactResponse> UpdateStatus(UpdateContactStatusRequest updateContactStatusRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/status"))
            {
                return await PostPutRequestWithResponseAsync<UpdateContactStatusRequest, ContactResponse>(request, updateContactStatusRequest);
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

        public async Task<List<ContactResponse>> GetAllContactsForOrganisation(string epaoId, bool? withUser = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts/getAll"))
            {
                var response = await PostPutRequestWithResponseAsync<GetAllContactsRequest, List<ContactResponse>>(request,
                        new GetAllContactsRequest(epaoId, withUser));

                return response;
            }
        }

        public async Task<List<ContactIncludePrivilegesResponse>> GetAllContactsForOrganisationIncludePrivileges(string epaoId, bool? withUser = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts/getAll/includePrivileges"))
            {
                var response = await PostPutRequestWithResponseAsync<GetAllContactsIncludePrivilegesRequest, List<ContactIncludePrivilegesResponse>>(request,
                        new GetAllContactsIncludePrivilegesRequest(epaoId, withUser));

                return response;
            }
        }

        public async Task<List<ContactResponse>> GetAllContactsWhoCanBePrimaryForOrganisation(string epaoId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/getAllWhoCanBePrimary/{epaoId}"))
            {
                return await RequestAndDeserialiseAsync<List<ContactResponse>>(request, $"Could not find any contacts who can be primary for organisation {epaoId}");
            }
        }

        public async Task<ContactResponse> UpdateOrgAndStatus(UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStausRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/updateContactWithOrgAndStatus"))
            {
                return await PostPutRequestWithResponseAsync<UpdateContactWithOrgAndStausRequest, ContactResponse>(request, updateContactWithOrgAndStausRequest);
            }
        }

        public async Task<ContactBoolResponse> InviteUser(CreateContactRequest createContactRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts"))
            {
                var response =
                    await PostPutRequestWithResponseAsync<CreateContactRequest, ContactBoolResponse>(request,
                        createContactRequest);

                return response;
            }
        }

        public async Task Callback(SignInCallback callback)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts/callback"))
            {
                await PostPutRequestAsync(request, callback);
            }
        }

        public async Task MigrateSingleContactToApply(System.Guid signinId)
        {
            var signinIdWrapper = new SigninIdWrapper(signinId);
            _logger.LogInformation($"MigrateSingleContactToApply json being POSTed: {JsonConvert.SerializeObject(signinIdWrapper)}");
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/MigrateSingleContactToApply"))
            {
                await PostPutRequestAsync(request, signinIdWrapper);
            }
        }

        public async Task<ContactResponse> CreateANewContactWithGivenId(Contact contact)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/createNewContactWithGivenId"))
            {
                var response =
                     await PostPutRequestWithResponseAsync<Contact,ContactResponse>(request,contact);

                return response;
            }
        }

        public async Task AssociateDefaultRolesAndPrivileges(Contact contact)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/associateDefaultRolesAndPrivileges"))
            {
                 await PostPutRequestAsync(request, contact);
                
            }
        }

        public async Task<SetContactPrivilegesResponse> SetContactPrivileges(SetContactPrivilegesRequest privilegesRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/setContactPrivileges"))
            {
                return await PostPutRequestWithResponseAsync<SetContactPrivilegesRequest,SetContactPrivilegesResponse>(request, privilegesRequest);
            }
        }

        public async Task<RemoveContactFromOrganisationResponse> RemoveContactFromOrganisation(Guid requestingUserId, Guid contactId)
        {
            var removeContactRequest = new RemoveContactFromOrganisationRequest(requestingUserId, contactId);
            
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/removeContactFromOrganisation"))
            {
                return await PostPutRequestWithResponseAsync<RemoveContactFromOrganisationRequest,RemoveContactFromOrganisationResponse>(request, removeContactRequest);
            }
        }

        public async Task<InviteContactToOrganisationResponse> InviteContactToOrganisation(InviteContactToOrganisationRequest invitationRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/inviteContactToOrganisation"))
            {
                return await PostPutRequestWithResponseAsync<InviteContactToOrganisationRequest,InviteContactToOrganisationResponse>(request, invitationRequest);
            }
        }

        public async Task RequestForPrivilege(Guid contactId, Guid privilegeId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/requestForPrivilege"))
            {
                await PostPutRequestAsync(request, new RequestForPrivilegeRequest {ContactId = contactId, PrivilegeId = privilegeId});
            }
        }

        public async Task ApproveContact(Guid contactId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/approve"))
            {
                await PostPutRequestAsync(request, new ApproveContactRequest {ContactId = contactId});
            }
        }
        
        public async Task RejectContact(Guid contactId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/reject"))
            {
                await PostPutRequestAsync(request, new RejectContactRequest {ContactId = contactId});
            }
        }

        public async Task<ContactResponse> GetContactByEmail(string emailAddress)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/email/{WebUtility.UrlEncode(emailAddress)}"))
            {
                return await RequestAndDeserialiseAsync<ContactResponse>(request, $"Could not find the contact");
            }
        }

        public async Task UpdateEmail(UpdateEmailRequest updateEmailRequest)
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/updateEmail");
            await PostPutRequestAsync(request, updateEmailRequest);
        }
    }
}