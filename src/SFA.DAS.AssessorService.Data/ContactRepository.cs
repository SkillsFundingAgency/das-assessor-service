using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        private readonly IDbConnection _connection;

        public ContactRepository(AssessorDbContext assessorDbContext, IDbConnection connection)
        {
            _assessorDbContext = assessorDbContext;
            _connection = connection;
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
                var contactPrivilegeEntity =
                    await _assessorDbContext.ContactsPrivileges.FirstOrDefaultAsync(q =>
                        q.ContactId == contactId && q.PrivilegeId == privilege.Id);
                if (contactPrivilegeEntity == null)
                {
                    _assessorDbContext.ContactsPrivileges.Add(new ContactsPrivilege
                    {
                        ContactId = contactId,
                        PrivilegeId = privilege.Id
                    });
                }
            }

            await _assessorDbContext.SaveChangesAsync();
        }

        public bool CheckIfAnyPrivelegesSet(Guid contactId)
        {
            var result = false;
            if (_assessorDbContext.ContactsPrivileges != null)
                result = _assessorDbContext.ContactsPrivileges.Any(x =>
                    x.ContactId == contactId);
            return result;
        }

        public async Task RemoveAllPrivileges(Guid contactId)
        {
            _assessorDbContext.ContactsPrivileges.RemoveRange(_assessorDbContext.ContactsPrivileges.Where(cp => cp.ContactId == contactId));
            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task AddPrivilege(Guid contactId, Guid privilegeId)
        {
            await _assessorDbContext.ContactsPrivileges.AddAsync(new ContactsPrivilege() {ContactId = contactId, PrivilegeId = privilegeId});
            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task<bool> IsOnlyContactWithPrivilege(Guid contactId, Guid privilegeId)
        {
            var contact = await _assessorDbContext.Contacts.FirstOrDefaultAsync(c => c.Id == contactId);
            var orgContacts = _assessorDbContext.Contacts.Where(c => c.OrganisationId == contact.OrganisationId).Select(c => c.Id);

            var orgContactPrivileges = await _assessorDbContext.ContactsPrivileges.Where(cp => orgContacts.Contains(cp.ContactId)).ToListAsync();

            return orgContactPrivileges.Count(ocp => ocp.PrivilegeId == privilegeId) == 1;
        }

        public async Task CreateContactLog(Guid userId, Guid contactId, string logType, object logData)
        {
            await _connection.ExecuteAsync(@"INSERT INTO ContactLogs (DateTime, UserId, ContactId, ContactLogType, ContactLogDetails) 
                                                                   VALUES (GETUTCDATE(), @userId, @contactId, @logType, @logDataString)"
                , new {userId, contactId, logType, logDataString = logData == null ? "" : JsonConvert.SerializeObject(logData)});
        }

        public async Task RemoveContactFromOrganisation(Guid contactId)
        {
            await _connection.ExecuteAsync("UPDATE Contacts SET OrganisationId = NULL, EndPointAssessorOrganisationId = NULL, Status = 'New' WHERE Id = @contactId", new {contactId});
        }

        public async Task UpdateOrganisationId(Guid contactId, Guid? organisationId)
        {
            await _connection.ExecuteAsync("UPDATE Contacts SET OrganisationId = @organisationId WHERE Id = @contactId", new {contactId, organisationId});
        }

        public async Task AddContactInvitation(Guid invitorContactId, Guid inviteeContactId, Guid organisationId)
        {
            var contactInvitation = new ContactInvitation() {InvitationDate = DateTime.UtcNow, OrganisationId = organisationId, InvitorContactId = invitorContactId, InviteeContactId = inviteeContactId};
            await _assessorDbContext.ContactInvitations.AddAsync(contactInvitation);
            await _assessorDbContext.SaveChangesAsync();
        }

        public Task<ContactInvitation> GetContactInvitation(Guid inviteeContactId)
        {
            return await _assessorDbContext.ContactInvitations.SingleOrDefaultAsync(ci => ci.InviteeContactId == inviteeContactId);
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
                await _assessorDbContext.Contacts.FirstAsync(q => q.Id == updateContactStatusRequest.Id);

            contactEntity.Status = updateContactStatusRequest.Status;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task UpdateStatus(Guid contactId, string status)
        {
            var contactEntity =
                await _assessorDbContext.Contacts.FirstAsync(c => c.Id == contactId);

            contactEntity.Status = status;
            
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

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task<Contact> GetContact(string email)
        {
            return  await _assessorDbContext.Contacts.FirstOrDefaultAsync(q => q.Email == email);
        }

        public async Task UpdateUserName(Guid contactId, string userName)
        {
            var contactEntity =
               await _assessorDbContext.Contacts.FirstAsync(q => q.Id == contactId);

            contactEntity.Username=userName;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }
    }
}