using System;
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

        public async Task Update(UpdateContactRequest updateContactRequest)
        {
            var contactEntity = await _assessorDbContext.Contacts.FirstOrDefaultAsync(q => q.Username == updateContactRequest.UserName 
                                                                                        || q.Email == updateContactRequest.Email);
            if (contactEntity == null)
                throw new NotFound();

            contactEntity.Username = updateContactRequest.UserName;
            contactEntity.DisplayName = updateContactRequest.DisplayName;
            contactEntity.Email = updateContactRequest.Email;

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
    }
}