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

        Task<List<ContactsWithPrivilegesResponse>> GetContactsWithPrivileges(Guid organisationId);

        Task<ContactBoolResponse> DoesContactHavePrivileges(string userId);

        Task<ContactResponse> UpdateStatus(UpdateContactStatusRequest updateContactStatusRequest);

        Task<ContactResponse> GetById(string id);

        Task<ContactResponse> GetContactBySignInId(string signInId);
        Task<List<ContactResponse>> GetAllContactsForOrganisation(string epaoId);

        Task<ContactResponse> UpdateOrgAndStatus(
            UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStausRequest);

        Task<ContactBoolResponse> InviteUser(CreateContactRequest createAccountRequest);
        Task Callback(DfeSignInCallback callback);

        Task MigrateUsers();

        Task MigrateContactsAndOrgsToApply();

        Task MigrateSingleContactToApply(System.Guid signinId);

        Task<ContactResponse> CreateANewContactWithGivenId(Contact contact);

        Task AssociateDefaultRolesAndPrivileges(Contact contact);
        Task<SetContactPrivilegesResponse> SetContactPrivileges(SetContactPrivilegesRequest privilegesRequest);
        Task<RemoveContactFromOrganisationResponse> RemoveContactFromOrganisation(Guid requestingUserId, Guid contactId);
        Task<InviteContactToOrganisationResponse> InviteContactToOrganisation(InviteContactToOrganisationRequest request);
    }
}