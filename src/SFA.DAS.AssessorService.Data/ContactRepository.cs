namespace SFA.DAS.AssessorService.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Application.Domain;
    using Application.Interfaces;
    using AutoMapper;
    using Domain.Consts;
    using Domain.Exceptions;
    using Microsoft.EntityFrameworkCore;

    public class ContactRepository : IContactRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public ContactRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<Contact> CreateNewContact(ContactCreateDomainModel newContact)
        {
            var contactEntity = Mapper.Map<Domain.Entities.Contact>(newContact);

            _assessorDbContext.Contacts.Add(contactEntity);
            await _assessorDbContext.SaveChangesAsync();

            var contactQueryViewModel = Mapper.Map<Contact>(contactEntity);
            return contactQueryViewModel;
        }

        public async Task Update(UpdateContactRequest contactUpdateViewModel)
        {
            var contactEntity = await _assessorDbContext.Contacts.FirstAsync(q => q.Username == contactUpdateViewModel.Username);

            contactEntity.DisplayName = contactUpdateViewModel.DisplayName;
            contactEntity.Email = contactUpdateViewModel.Email;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task Delete(string userName)
        {
            var contactEntity = _assessorDbContext.Contacts
                      .FirstOrDefault(q => q.Username == userName);

            if (contactEntity == null)
                throw (new NotFound());

            contactEntity.DeletedAt = DateTime.Now;
            contactEntity.Status = ContactStatus.Deleted;

            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }
    }
}