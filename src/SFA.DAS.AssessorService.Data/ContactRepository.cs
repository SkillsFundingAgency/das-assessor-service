﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Data
{
    public class ContactRepository : Repository, IContactRepository
    {
        private readonly IAssessorUnitOfWork _assessorUnitOfWork;

        public ContactRepository(IAssessorUnitOfWork assessorUnitOfWork, IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            _assessorUnitOfWork = assessorUnitOfWork;
        }

        public async Task<Contact> CreateNewContact(Contact newContact)
        {
            _assessorUnitOfWork.AssessorDbContext.Contacts.Add(newContact);
            await _assessorUnitOfWork.SaveChangesAsync();

            return newContact;
        }
        
        public async Task AssociatePrivilegesWithContact(Guid contactId, IEnumerable<Privilege> privileges)
        {
            foreach (var privilege in privileges)
            {
                var contactPrivilegeEntity =
                    await _assessorUnitOfWork.AssessorDbContext.ContactsPrivileges.FirstOrDefaultAsync(q =>
                        q.ContactId == contactId && q.PrivilegeId == privilege.Id);
                if (contactPrivilegeEntity == null)
                {
                    _assessorUnitOfWork.AssessorDbContext.ContactsPrivileges.Add(new ContactsPrivilege
                    {
                        ContactId = contactId,
                        PrivilegeId = privilege.Id
                    });
                }
            }

            await _assessorUnitOfWork.SaveChangesAsync();
        }

        public bool CheckIfAnyPrivelegesSet(Guid contactId)
        {
            var result = false;
            if (_assessorUnitOfWork.AssessorDbContext.ContactsPrivileges != null)
                result = _assessorUnitOfWork.AssessorDbContext.ContactsPrivileges.Any(x =>
                    x.ContactId == contactId);
            return result;
        }

        public async Task RemoveAllPrivileges(Guid contactId)
        {
            _assessorUnitOfWork.AssessorDbContext.ContactsPrivileges.RemoveRange(_assessorUnitOfWork.AssessorDbContext.ContactsPrivileges.Where(cp => cp.ContactId == contactId));
            await _assessorUnitOfWork.SaveChangesAsync();
        }

        public async Task AddPrivilege(Guid contactId, Guid privilegeId)
        {
            await _assessorUnitOfWork.AssessorDbContext.ContactsPrivileges.AddAsync(new ContactsPrivilege() {ContactId = contactId, PrivilegeId = privilegeId});
            await _assessorUnitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsOnlyContactWithPrivilege(Guid contactId, Guid privilegeId)
        {
            var contact = await _assessorUnitOfWork.AssessorDbContext.Contacts.FirstOrDefaultAsync(c => c.Id == contactId);
            var orgContacts = _assessorUnitOfWork.AssessorDbContext.Contacts.Where(c => c.OrganisationId == contact.OrganisationId).Select(c => c.Id);

            var orgContactPrivileges = await _assessorUnitOfWork.AssessorDbContext.ContactsPrivileges.Where(cp => orgContacts.Contains(cp.ContactId)).ToListAsync();

            return orgContactPrivileges.Count(ocp => ocp.PrivilegeId == privilegeId) == 1;
        }

        public async Task CreateContactLog(Guid userId, Guid contactId, string logType, object logData)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                @"INSERT INTO ContactLogs (DateTime, UserId, ContactId, ContactLogType, ContactLogDetails) 
                  VALUES (GETUTCDATE(), @userId, @contactId, @logType, @logDataString)"
                , new { userId, contactId, logType, logDataString = logData == null ? "" : JsonConvert.SerializeObject(logData) });
        }

        public async Task RemoveContactFromOrganisation(Guid contactId)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE Contacts SET OrganisationId = NULL, EndPointAssessorOrganisationId = NULL, Status = 'New' WHERE Id = @contactId", 
                new { contactId });
        }

        public async Task UpdateOrganisationId(Guid contactId, Guid? organisationId)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE Contacts SET OrganisationId = @organisationId WHERE Id = @contactId", 
                new { contactId, organisationId });
        }

        public async Task AddContactInvitation(Guid invitorContactId, Guid inviteeContactId, Guid organisationId)
        {
            var contactInvitation = new ContactInvitation() {InvitationDate = DateTime.UtcNow, OrganisationId = organisationId, InvitorContactId = invitorContactId, InviteeContactId = inviteeContactId};
            await _assessorUnitOfWork.AssessorDbContext.ContactInvitations.AddAsync(contactInvitation);
            await _assessorUnitOfWork.SaveChangesAsync();
        }

        public async Task<ContactInvitation> GetContactInvitation(Guid inviteeContactId)
        {
            return await _assessorUnitOfWork.AssessorDbContext.ContactInvitations.SingleOrDefaultAsync(ci => ci.InviteeContactId == inviteeContactId);
        }

        public async Task SetInvitationAccepted(ContactInvitation contactInvitation)
        {
            contactInvitation.IsAccountCreated = true;
            contactInvitation.AccountCreatedDate = DateTime.UtcNow;
            await _assessorUnitOfWork.SaveChangesAsync();
        }

        public async Task Update(UpdateContactRequest updateContactRequest)
        {
            var contactEntity = await _assessorUnitOfWork.AssessorDbContext.Contacts.FirstOrDefaultAsync(q => q.Username == updateContactRequest.UserName);

            if(contactEntity == null)
                contactEntity = await _assessorUnitOfWork.AssessorDbContext.Contacts.OrderBy(q => q.CreatedAt).FirstOrDefaultAsync(q => q.Email == updateContactRequest.Email);

            if (contactEntity == null)
                throw new NotFoundException();

            contactEntity.Username = updateContactRequest.UserName;
            contactEntity.DisplayName = updateContactRequest.DisplayName;
            contactEntity.Email = updateContactRequest.Email;

            // Workaround for Mocking
            _assessorUnitOfWork.AssessorDbContext.MarkAsModified(contactEntity);

            await _assessorUnitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatus(UpdateContactStatusRequest updateContactStatusRequest)
        {
            var contactEntity =
                await _assessorUnitOfWork.AssessorDbContext.Contacts.FirstAsync(q => q.Id == updateContactStatusRequest.Id);

            contactEntity.Status = updateContactStatusRequest.Status;

            // Workaround for Mocking
            _assessorUnitOfWork.AssessorDbContext.MarkAsModified(contactEntity);

            await _assessorUnitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStatus(Guid contactId, string status)
        {
            var contactEntity =
                await _assessorUnitOfWork.AssessorDbContext.Contacts.FirstAsync(c => c.Id == contactId);

            contactEntity.Status = status;
            
            await _assessorUnitOfWork.SaveChangesAsync();
        }

        public async Task<Contact> UpdateContactWithOrganisationData(UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStaus)
        {
            var contactEntity =
                await _assessorUnitOfWork.AssessorDbContext.Contacts.FirstAsync(q => q.Id == Guid.Parse(updateContactWithOrgAndStaus.ContactId));
            contactEntity.Status = updateContactWithOrgAndStaus.Status;
            contactEntity.OrganisationId = Guid.Parse(updateContactWithOrgAndStaus.OrgId);
            contactEntity.EndPointAssessorOrganisationId = updateContactWithOrgAndStaus.EpaOrgId;

            // Workaround for Mocking
            _assessorUnitOfWork.AssessorDbContext.MarkAsModified(contactEntity);

            await _assessorUnitOfWork.SaveChangesAsync();

            //return updated contact
            return await _assessorUnitOfWork.AssessorDbContext.Contacts.FirstAsync(q => q.Id == Guid.Parse(updateContactWithOrgAndStaus.ContactId));
        }

        public async Task LinkOrganisation(string endPointAssessorOrganisationId, string userName)
        {
            var organisationEntity = await _assessorUnitOfWork.AssessorDbContext.Organisations.FirstAsync(q =>
                q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId);

            var contactEntity =
                await _assessorUnitOfWork.AssessorDbContext.Contacts.FirstAsync(q => q.Username == userName);

            contactEntity.OrganisationId = organisationEntity.Id;

            // Workaround for Mocking
            _assessorUnitOfWork.AssessorDbContext.MarkAsModified(contactEntity);

            await _assessorUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(string userName)
        {
            var contactEntity = _assessorUnitOfWork.AssessorDbContext.Contacts
                .FirstOrDefault(q => q.Username == userName);

            if (contactEntity == null)
                throw new NotFoundException();

            // Ignore if already deleted
            if (contactEntity.Status == ContactStatus.Deleted)
                return;

            contactEntity.DeletedAt = DateTime.Now;
            contactEntity.Status = ContactStatus.Deleted;

            _assessorUnitOfWork.AssessorDbContext.MarkAsModified(contactEntity);

            await _assessorUnitOfWork.SaveChangesAsync();
        }

        public async Task<Contact> UpdateGovUkIdentifier(Guid contactId, string govIdentifier)
        {
            var contactEntity =
                await _assessorUnitOfWork.AssessorDbContext.Contacts.FirstAsync(q => q.Id == contactId);

            if (!string.IsNullOrEmpty(govIdentifier))
            {
                contactEntity.GovUkIdentifier = govIdentifier;
                if (string.IsNullOrEmpty(contactEntity.SignInType))
                {
                    contactEntity.SignInType = "GovLogin";    
                }
            }

            // Workaround for Mocking
            _assessorUnitOfWork.AssessorDbContext.MarkAsModified(contactEntity);

            await _assessorUnitOfWork.SaveChangesAsync();
            return contactEntity;
        }

        public async Task<Contact> GetContact(string email)
        {
            return  await _assessorUnitOfWork.AssessorDbContext.Contacts.FirstOrDefaultAsync(q => q.Email == email);
        }

        public async Task UpdateUserName(Guid contactId, string userName)
        {
            var contactEntity =
               await _assessorUnitOfWork.AssessorDbContext.Contacts.FirstAsync(q => q.Id == contactId);

            contactEntity.Username=userName;

            // Workaround for Mocking
            _assessorUnitOfWork.AssessorDbContext.MarkAsModified(contactEntity);

            await _assessorUnitOfWork.SaveChangesAsync();
        }

        public async Task UpdateEmail(UpdateEmailRequest request)
        {
            var contactEntity = await _assessorUnitOfWork.AssessorDbContext.Contacts.FirstOrDefaultAsync(q => q.GovUkIdentifier == request.GovUkIdentifier);

            if (contactEntity == null || contactEntity.Email == request.NewEmail)
            {
                return;
            }
            
            contactEntity.Email = request.NewEmail;
            contactEntity.Username = request.NewEmail;

            // Workaround for Mocking
            _assessorUnitOfWork.AssessorDbContext.MarkAsModified(contactEntity);

            await _assessorUnitOfWork.SaveChangesAsync();
        }
    }
}