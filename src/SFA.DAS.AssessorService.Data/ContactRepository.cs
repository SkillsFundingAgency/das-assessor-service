using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Data
{
    public class ContactRepository : IContactRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public ContactRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<Contact> CreateNewContact(Contact newContact)
        {
            _assessorDbContext.Contacts.Add(newContact);
            await _assessorDbContext.SaveChangesAsync();

            return newContact;
        }
        

        public async Task AssociateRoleWithContact(string roleName, Contact newContact)
        {
            var contactRoleEntity =
                await _assessorDbContext.ContactRoles.FirstOrDefaultAsync(q =>
                    q.ContactId == newContact.Id && q.RoleName == roleName);
            if (contactRoleEntity == null)
            {
                _assessorDbContext.ContactRoles.Add(new ContactRole
                {
                    ContactId = newContact.Id,
                    Id = Guid.NewGuid(),
                    RoleName = roleName
                });
                await _assessorDbContext.SaveChangesAsync();
            }
        }

        public async Task AssociatePrivilegesWithContact(Guid contactId, IEnumerable<Privilege> privileges)
        {
            foreach (var privilege in privileges)
            {
                _assessorDbContext.ContactsPrivileges.Add(new ContactsPrivilege
                {
                    ContactId = contactId,
                    PrivilegeId = privilege.Id
                });
            }
            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task Update(UpdateContactRequest updateContactRequest)
        {
            var contactEntity = await _assessorDbContext.Contacts.FirstOrDefaultAsync(q => q.Username == updateContactRequest.UserName);

            if(contactEntity == null)
                contactEntity = await _assessorDbContext.Contacts.OrderBy(q => q.CreatedAt).FirstOrDefaultAsync(q => q.Email == updateContactRequest.Email);

            if (contactEntity == null)
                throw new NotFound();

            contactEntity.Username = updateContactRequest.UserName;
            contactEntity.DisplayName = updateContactRequest.DisplayName;
            contactEntity.Email = updateContactRequest.Email;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task UpdateStatus(UpdateContactStatusRequest updateContactStatusRequest)
        {
            var contactEntity =
                await _assessorDbContext.Contacts.FirstAsync(q => q.Id == Guid.Parse(updateContactStatusRequest.Id));

            contactEntity.Status = updateContactStatusRequest.Status == ContactStatus.Approve
                ? ContactStatus.Live
                : (updateContactStatusRequest.Status == ContactStatus.Applying
                    ? ContactStatus.Applying
                    : ContactStatus.Inactive);

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task<Contact> UpdateContactWithOrganisationData(UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStaus)
        {
            var contactEntity =
                await _assessorDbContext.Contacts.FirstAsync(q => q.Id == Guid.Parse(updateContactWithOrgAndStaus.ContactId));
            contactEntity.Status = updateContactWithOrgAndStaus.Status;
            contactEntity.OrganisationId = Guid.Parse(updateContactWithOrgAndStaus.OrgId);
            contactEntity.EndPointAssessorOrganisationId = updateContactWithOrgAndStaus.EpaOrgId;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();

            //return updated contact
            return await _assessorDbContext.Contacts.FirstAsync(q => q.Id == Guid.Parse(updateContactWithOrgAndStaus.ContactId));
        }

        public async Task LinkOrganisation(string endPointAssessorOrganisationId, string userName)
        {
            var organisationEntity = await _assessorDbContext.Organisations.FirstAsync(q =>
                q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId);

            var contactEntity =
                await _assessorDbContext.Contacts.FirstAsync(q => q.Username == userName);

            contactEntity.OrganisationId = organisationEntity.Id;            

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task Delete(string userName)
        {
            var contactEntity = _assessorDbContext.Contacts
                .FirstOrDefault(q => q.Username == userName);

            if (contactEntity == null)
                throw new NotFound();

            // Ignore if already deleted
            if (contactEntity.Status == ContactStatus.Deleted)
                return;

            contactEntity.DeletedAt = DateTime.Now;
            contactEntity.Status = ContactStatus.Deleted;

            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task UpdateSignInId(Guid contactId, Guid? signInId)
        {
            var contactEntity =
                await _assessorDbContext.Contacts.FirstAsync(q => q.Id == contactId);

            contactEntity.SignInId = signInId;
            contactEntity.Status = ContactStatus.New;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task<Contact> GetContact(string email)
        {
           
            return  await _assessorDbContext.Contacts.FirstOrDefaultAsync(q => q.Email == email);

        }


    }
}