namespace SFA.DAS.AssessorService.Data
{
    using System;
    using System.Collections.Generic;
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

        public async Task<IEnumerable<ContactQueryViewModel>> GetContacts(Guid id)
        {
            var contacts = await _assessorDbContext.Organisations
                .Include(organisation => organisation.Contacts)
                .Where(organisation => organisation.Id == id)
                .SelectMany(q => q.Contacts).Where(q => q.ContactStatus == ContactStatus.Live)
                .Select(contact => Mapper.Map<ContactQueryViewModel>(contact)).ToListAsync();

            return contacts;
        }

        public async Task<ContactQueryViewModel> GetContact(string userName, string emailAddress)
        {
            var contact = await _assessorDbContext.Contacts
                .FirstOrDefaultAsync(q => q.ContactName == userName && q.ContactEmail == emailAddress && q.ContactStatus != ContactStatus.Deleted);
            if (contact == null)
                throw new NotFound();

            var contactQueryViewModel = Mapper.Map<ContactQueryViewModel>(contact);
            return contactQueryViewModel;
        }

        public async Task<bool> CheckContactExists(int contactId)
        {
            var result = await _assessorDbContext.Contacts
                         .AnyAsync(q => q.EndPointAssessorContactId == contactId && q.ContactStatus != ContactStatus.Deleted);
            return result;
        }

        public async Task<ContactQueryViewModel> CreateNewContact(ContactCreateDomainModel newContact)
        {
            var contactEntity = Mapper.Map<Contact>(newContact);

            _assessorDbContext.Contacts.Add(contactEntity);
            await _assessorDbContext.SaveChangesAsync();

            var organisationQueryViewModel = Mapper.Map<ContactQueryViewModel>(contactEntity);
            return organisationQueryViewModel;
        }
    }
}