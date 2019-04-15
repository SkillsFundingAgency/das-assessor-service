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

        Task<Contact> UpdateContactWithOrganisationData(
            UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStaus);

        Task UpdateSignInId(Guid contactId, Guid? signInId);
        Task<Contact> GetContact(string email);
        Task AssociateRoleWithContact(string roleName, Contact newContact);
        Task AssociatePrivilegesWithContact(Guid contactId, IEnumerable<Privilege> privileges);
    }
}