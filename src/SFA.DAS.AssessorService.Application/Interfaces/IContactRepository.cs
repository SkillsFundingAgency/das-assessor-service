using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{ 
    public interface IContactRepository
    {       
        Task<Contact> CreateNewContact(Contact newContact);
        Task Update(UpdateContactRequest updateContactRequest);
        Task Delete(string userName);
        Task LinkOrganisation(string endPointAssessorOrganisationId, string userName);
        Task UpdateStatus(UpdateContactStatusRequest updateContactRequest);
        Task UpdateStatus(Guid contactId, string status);

        Task<Contact> UpdateContactWithOrganisationData(
            UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStaus);

        Task UpdateSignInId(Guid contactId, Guid? signInId);
        Task<Contact> GetContact(string email);
        Task AssociateRoleWithContact(string roleName, Contact newContact);
        Task AssociatePrivilegesWithContact(Guid contactId, IEnumerable<Privilege> privileges);
        bool CheckIfAnyPrivelegesSet(Guid contactId);
        Task UpdateUserName(Guid contactId, string userName);
        Task RemoveAllPrivileges(Guid contactId);
        Task AddPrivilege(Guid contactId, Guid privilegeId);
        Task<bool> IsOnlyContactWithPrivilege(Guid contactId, Guid privilegeId);
        Task CreateContactLog(Guid userId, Guid contactId, string logType, object logData);
        Task RemoveContactFromOrganisation(Guid contactId);
        Task UpdateOrganisationId(Guid contactId, Guid? organisationId);
        Task AddContactInvitation(Guid invitorContactId, Guid inviteeContactId, Guid organisationId);
        Task<ContactInvitation> GetContactInvitation(Guid inviteeContactId);
        Task SetInvitationAccepted(ContactInvitation contactInvitation);
    }
}