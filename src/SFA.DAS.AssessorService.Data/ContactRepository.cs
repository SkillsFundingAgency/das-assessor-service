using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DomainModels;
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

        public async Task<ContactResponse> CreateNewContact(CreateContactDomainModel newContact)
        {
            var contactEntity = Mapper.Map<Contact>(newContact);

            _assessorDbContext.Contacts.Add(contactEntity);
            await _assessorDbContext.SaveChangesAsync();

            var contact = Mapper.Map<ContactResponse>(contactEntity);
            return contact;
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

        public async Task Delete(string userName)
        {
            var contactEntity = _assessorDbContext.Contacts
                .FirstOrDefault(q => q.Username == userName);

            if (contactEntity == null)
                throw new NotFound();

            contactEntity.DeletedAt = DateTime.Now;
            contactEntity.Status = ContactStatus.Deleted;

            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }
    }
}