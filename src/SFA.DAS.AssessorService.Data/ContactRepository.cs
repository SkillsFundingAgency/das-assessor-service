﻿namespace SFA.DAS.AssessorService.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Entities;
    using SFA.DAS.AssessorService.Domain.Enums;
    using SFA.DAS.AssessorService.Domain.Exceptions;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class ContactRepository : IContactRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public ContactRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<ContactQueryViewModel> CreateNewContact(ContactCreateDomainModel newContact)
        {
            var contactEntity = Mapper.Map<Contact>(newContact);

            _assessorDbContext.Contacts.Add(contactEntity);
            await _assessorDbContext.SaveChangesAsync();

            var contactQueryViewModel = Mapper.Map<ContactQueryViewModel>(contactEntity);
            return contactQueryViewModel;
        }

        public async Task Update(ContactUpdateViewModel contactUpdateViewModel)
        {
            var contactEntity = await _assessorDbContext.Contacts.FirstAsync(q => q.Id == contactUpdateViewModel.Id);

            contactEntity.ContactName = contactUpdateViewModel.ContactName;
            contactEntity.ContactEmail = contactUpdateViewModel.ContactEmail;

            // Workaround for Mocking
            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var contactEntity = _assessorDbContext.Contacts
                      .FirstOrDefault(q => q.Id == id);

            if (contactEntity == null)
                throw (new NotFound());

            contactEntity.DeletedAt = DateTime.Now;
            contactEntity.ContactStatus = ContactStatus.Deleted;

            _assessorDbContext.MarkAsModified(contactEntity);

            await _assessorDbContext.SaveChangesAsync();
        }
    }
}