using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IContactsApiClient
    {
        Task<List<Privilege>> GetPrivileges();
        
        Task<List<ContactsPrivilege>> GetContactPrivileges(Guid userId);
        
        Task<ContactResponse> GetByUsername(string username);
        
        Task<ContactResponse> Update(UpdateContactRequest updateContactRequest);

        

        Task<ContactBoolResponse> DoesContactHavePrivileges(string userId);

        Task<ContactResponse> UpdateStatus(UpdateContactStatusRequest updateContactStatusRequest);

        Task<ContactResponse> GetById(Guid id);

        Task<ContactResponse> GetContactBySignInId(string signInId);

        Task<List<ContactResponse>> GetAllContactsForOrganisation(string epaoId, bool? withUser = null);
        Task<List<ContactIncludePrivilegesResponse>> GetAllContactsForOrganisationIncludePrivileges(string epaoId, bool? withUser = null);

        Task<List<ContactResponse>> GetAllContactsWhoCanBePrimaryForOrganisation(string epaoId);
        
        Task<ContactResponse> UpdateOrgAndStatus(
            UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStausRequest);

        Task<ContactBoolResponse> InviteUser(CreateContactRequest createAccountRequest);
        Task Callback(SignInCallback callback);

        Task MigrateUsers();

        Task MigrateSingleContactToApply(System.Guid signinId);

        Task<ContactResponse> CreateANewContactWithGivenId(Contact contact);

        Task AssociateDefaultRolesAndPrivileges(Contact contact);
        Task<SetContactPrivilegesResponse> SetContactPrivileges(SetContactPrivilegesRequest privilegesRequest);
        Task<RemoveContactFromOrganisationResponse> RemoveContactFromOrganisation(Guid requestingUserId, Guid contactId);
        Task<InviteContactToOrganisationResponse> InviteContactToOrganisation(InviteContactToOrganisationRequest request);
        Task RequestForPrivilege(Guid contactId, Guid privilegeId);
        Task ApproveContact(Guid contactId);
        Task RejectContact(Guid contactId);
        Task<ContactResponse> GetContactByEmail(string emailAddress);
    }
}