using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class ContactQueryRepository : IContactQueryRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public ContactQueryRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<IEnumerable<Contact>> GetContacts(string endPointAssessorOrganisationId)
        {
            var contacts = await _assessorDbContext.Organisations
                .Include(organisation => organisation.Contacts)
                .Where(organisation => organisation.EndPointAssessorOrganisationId == endPointAssessorOrganisationId
                                       && organisation.Status != OrganisationStatus.Deleted)
                .SelectMany(q => q.Contacts).ToListAsync();

            return contacts;
        }
        

        public async Task<IEnumerable<Contact>> GetAllContacts(string endPointAssessorOrganisationId)
        {
            var contacts = await _assessorDbContext.Organisations
                .Include(organisation => organisation.Contacts)
                .Where(organisation => organisation.EndPointAssessorOrganisationId == endPointAssessorOrganisationId)
                .SelectMany(q => q.Contacts).ToListAsync();

            return contacts;
        }

        public async Task<IEnumerable<IGrouping<Contact, ContactsPrivilege>>> GetAllContactsWithPrivileges(
            string endPointAssessorOrganisationId)
        {
            var groupedContactPrivileges = await _assessorDbContext.Organisations
                .Include(organisation => organisation.Contacts)
                .Where(organisation => organisation.EndPointAssessorOrganisationId == endPointAssessorOrganisationId)
                .SelectMany(q => q.Contacts).SelectMany(a => a.ContactsPrivileges.Select(c => new ContactsPrivilege
                {
                    ContactId = c.ContactId,
                    PrivilegeId = c.PrivilegeId,
                    Contact = c.Contact,
                    Privilege = c.Privilege
                })).GroupBy(x => x.Contact).ToListAsync();

            return groupedContactPrivileges;
        }
        public async Task<IEnumerable<Privilege>> GetAllPrivileges()
        {
            return await _assessorDbContext.Privileges.ToListAsync();
        }

        public async Task<Contact> GetContact(string userName)
        {
            var contact = await _assessorDbContext.Organisations
                .Include(organisation => organisation.Contacts)
                .Where(q => q.Status != OrganisationStatus.Deleted)
                .SelectMany(q => q.Contacts)
                .Where(q => q.Username == userName)
                .FirstOrDefaultAsync();

            return contact;
        }

        public async Task<Contact> GetContactFromEmailAddress(string email)
        {
            var contact = await _assessorDbContext.Organisations
                .Include(organisation => organisation.Contacts)
                .Where(q => q.Status != OrganisationStatus.Deleted)
                .SelectMany(q => q.Contacts)
                .Where(q => q.Email == email)
                .FirstOrDefaultAsync();

            return contact;
        }

      
        public async Task<Contact> GetBySignInId(Guid requestSignInId)
        {
            return await _assessorDbContext.Contacts.FirstOrDefaultAsync(c => c.SignInId == requestSignInId);
        }

        public async Task<IList<ContactRole>> GetRolesFor(Guid contactId)
        {
            return await _assessorDbContext.ContactRoles.Where(cr => cr.ContactId == contactId).ToListAsync();
        }

        public async Task<IList<ContactsPrivilege>> GetPrivilegesFor(Guid contactId)
        {
            return await _assessorDbContext.ContactsPrivileges.Where(cr => cr.ContactId == contactId).ToListAsync();
        }

        public async Task<bool> CheckContactExists(string userName)
        {
            var result = await _assessorDbContext.Contacts
                .AnyAsync(q => q.Username == userName);
            return result;
        }

        public async Task<string> GetContactStatus(string endPointAssessorOrganisationId, Guid signInId)
        {
            var contactStatus = await _assessorDbContext.Contacts.Where(x =>
                    x.EndPointAssessorOrganisationId == endPointAssessorOrganisationId && x.SignInId == signInId)
                .FirstOrDefaultAsync();

            return contactStatus?.Status;
        }

        public async Task<Contact> GetContactById(Guid id)
        {
            return await _assessorDbContext.Contacts.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Contact>> GetUsersToMigrate()
        {
            return await _assessorDbContext.Contacts.Where(c => 
                c.SignInId == null 
                && c.Status == "Live" 
                && !c.Username.StartsWith("unknown")
                && c.Organisation != null 
                && c.Organisation.Status == "Live").ToListAsync();
        }

        public async Task<List<Contact>> GetExsitingContactsToMigrateToApply()
        {
            return await _assessorDbContext.Contacts.Include( x => x.Organisation).Include(x => x.Organisation.OrganisationType).Where(c =>
                c.SignInId != null
                && c.Organisation != null
                && c.Organisation.OrganisationType != null).ToListAsync();
        }

        public async Task<Contact> GetSingleContactsToMigrateToApply(Guid requestSignInId)
        {
            return await _assessorDbContext.Contacts.Include(x => x.Organisation).Include(x => x.Organisation.OrganisationType).
                FirstOrDefaultAsync(c => c.SignInId == requestSignInId);
        }

        public async Task UpdateMigratedContact(Guid contactId, Guid signInId)
        {
            var contact = await _assessorDbContext.Contacts.SingleOrDefaultAsync(c => c.Id == contactId);
            if (contact != null)
            {
                contact.SignInId = signInId;
                await _assessorDbContext.SaveChangesAsync();
            }
        }
    }
}