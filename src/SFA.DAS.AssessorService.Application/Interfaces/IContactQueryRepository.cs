using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{ 
    public interface IContactQueryRepository
    {
        Task<bool> CheckContactExists(string userName);

        Task<IEnumerable<Contact>> GetContacts(string endPointAssessorOrganisationId);
        Task<Contact> GetContact(string userName);
        Task<Contact> GetContactFromEmailAddress(string email);
        Task<IEnumerable<Contact>> GetAllContacts(string endPointAssessorOrganisationId);
        Task<IEnumerable<IGrouping<Contact, ContactsPrivilege>>> GetAllContactsWithPrivileges(
            string endPointAssessorOrganisationId);
        Task<string> GetContactStatus(string endPointAssessorOrganisationId, Guid signInId);
        Task<Contact> GetContactById(Guid id);
        Task<Contact> GetBySignInId(Guid requestSignInId);
        Task<IList<ContactRole>> GetRolesFor(Guid contactId);
        Task<IEnumerable<Privilege>> GetAllPrivileges();
    }
}