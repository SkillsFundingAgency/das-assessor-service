using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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

        public async Task Update(UpdateContactRequest updateContactRequest)
        {
            var contactEntity =
                await _assessorDbContext.Contacts.FirstAsync(q => q.Username == updateContactRequest.UserName);

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

        public async Task UpdateContactWithOrganisationData(UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStaus)
        {
            var contactEntity =
                await _assessorDbContext.Contacts.FirstAsync(q => q.Id == Guid.Parse(updateContactWithOrgAndStaus.ContactId));
            contactEntity.Status = updateContactWithOrgAndStaus.Status;
            contactEntity.OrganisationId = Guid.Parse(updateContactWithOrgAndStaus.OrgId);
            contactEntity.EndPointAssessorOrganisationId = updateContactWithOrgAndStaus.EpaOrgId;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
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

        public async Task UpdateSignInId(Guid contactId, Guid signInId)
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