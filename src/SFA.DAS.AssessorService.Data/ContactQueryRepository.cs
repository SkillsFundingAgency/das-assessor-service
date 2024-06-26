﻿using System;
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

        public async Task<IEnumerable<Contact>> GetContactsForOrganisation(Guid organisationId)
        {
            var contacts = await _assessorDbContext.Organisations
                .Include(organisation => organisation.Contacts)
                .Where(organisation => organisation.Id == organisationId
                                       && organisation.Status != OrganisationStatus.Deleted)
                .SelectMany(q => q.Contacts).ToListAsync();

            return contacts;
        }

        public async Task<IEnumerable<Contact>> GetContactsForEpao(string endPointAssessorOrganisationId)
        {
            var contacts = await _assessorDbContext.Organisations
                .Include(organisation => organisation.Contacts)
                .Where(organisation => organisation.EndPointAssessorOrganisationId == endPointAssessorOrganisationId
                                       && organisation.Status != OrganisationStatus.Deleted)
                .SelectMany(q => q.Contacts).ToListAsync();

            return contacts;
        }
        

        public async Task<IEnumerable<Contact>> GetAllContacts(string endPointAssessorOrganisationId, bool? withUser = null)
        {
            var contacts = await _assessorDbContext.Contacts
                .Include(contact => contact.Organisation)
                .Where(contact =>
                    contact.Organisation.EndPointAssessorOrganisationId == endPointAssessorOrganisationId &&
                    (!withUser.HasValue || (withUser.Value && contact.GovUkIdentifier != null) || (!withUser.Value && contact.GovUkIdentifier == null)))
                .ToListAsync();

            return contacts;
        }

        public async Task<IEnumerable<Contact>> GetAllContactsIncludePrivileges(string endPointAssessorOrganisationId, bool? withUser = null)
        {
            var contacts = await _assessorDbContext.Contacts
                .Include(contact => contact.Organisation)
                .Include("ContactsPrivileges.Privilege")
                .Where(contact =>
                    contact.Organisation.EndPointAssessorOrganisationId == endPointAssessorOrganisationId &&
                    (!withUser.HasValue || (withUser.Value && contact.GovUkIdentifier != null) || (!withUser.Value && contact.GovUkIdentifier == null)))
                .OrderBy(c => c.FamilyName).ThenBy(c => c.GivenNames)
                .ToListAsync();
            
            return contacts;
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
            var contact = await _assessorDbContext.Contacts
                .Include(c => c.Organisation)
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower() && c.Organisation.Status != OrganisationStatus.Deleted);
            
            return contact;
        }
        public async Task<Contact> GetContactByGovIdentifier(string govIdentifier)
        {
            var contact = await _assessorDbContext.Contacts
                .Include(c => c.Organisation)
                .FirstOrDefaultAsync(c => c.GovUkIdentifier.ToLower() == govIdentifier.ToLower() && c.Organisation.Status != OrganisationStatus.Deleted);
            
            return contact;
        }

        public async Task<IList<ContactsPrivilege>> GetPrivilegesFor(Guid contactId)
        {
            return await _assessorDbContext.ContactsPrivileges.Where(cr => cr.ContactId == contactId).Include(cp => cp.Privilege).ToListAsync();   
        }

        public async Task<bool> CheckContactExists(string userName)
        {
            var result = await _assessorDbContext.Contacts
                .AnyAsync(q => q.Username == userName);
            return result;
        }

        public async Task<Contact> GetContactById(Guid id)
        {
            return await _assessorDbContext.Contacts.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Contact>> GetUsersToMigrate()
        {
            return await _assessorDbContext.Contacts.Where(c =>
                c.GovUkIdentifier == null
                && c.Status == "Live"
                && !c.Username.StartsWith("unknown")
                && c.Organisation != null
                && c.Organisation.Status == "Live").ToListAsync();
        }

        public async Task UpdateMigratedContact(Guid contactId, string govUkIdentifier)
        {
            var contact = await _assessorDbContext.Contacts.SingleOrDefaultAsync(c => c.Id == contactId);
            if (contact != null)
            {
                contact.GovUkIdentifier = govUkIdentifier;
                await _assessorDbContext.SaveChangesAsync();
            }
        }
    }
}