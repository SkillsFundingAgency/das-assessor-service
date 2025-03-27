using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Interfaces
{ 
    public interface IContactQueryRepository
    {
        Task<bool> CheckContactExists(string userName);

        Task<IEnumerable<Contact>> GetContactsForOrganisation(Guid organisationId);
        Task<IEnumerable<Contact>> GetContactsForEpao(string endPointAssessorOrganisationId);
        Task<Contact> GetContact(string userName);
        Task<Contact> GetContactFromEmailAddress(string email);
        Task<Contact> GetContactByGovIdentifier(string govIdentifier);
        Task<IEnumerable<Contact>> GetAllContacts(string endPointAssessorOrganisationId, bool? withUser = null);
        Task<IEnumerable<Contact>> GetAllContactsIncludePrivileges(string endPointAssessorOrganisationId, bool? withUser = null);
        Task<IList<ContactsPrivilege>> GetPrivilegesFor(Guid contactId);
        Task<IEnumerable<Privilege>> GetAllPrivileges();
        Task<Contact> GetContactById(Guid id);
        Task<List<Contact>> GetUsersToMigrate();
        Task UpdateMigratedContact(Guid contactId, string govUkIdentifier);
    }
}