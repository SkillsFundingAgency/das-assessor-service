namespace SFA.DAS.AssessorService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Enums;
    using SFA.DAS.AssessorService.Domain.Exceptions;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class ContactQueryRepository : IContactQueryRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public ContactQueryRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<IEnumerable<Contact>> GetContacts(string endPointAssessorOrganisationId)
        {
            var contacts = await _assessorDbContext.Organisations
                .Include(organisation => organisation.Contacts)
                .Where(organisation => organisation.EndPointAssessorOrganisationId == endPointAssessorOrganisationId)
                .SelectMany(q => q.Contacts).Where(q => q.ContactStatus == ContactStatus.Live)
                .Select(contact => Mapper.Map<Contact>(contact)).AsNoTracking().ToListAsync();

            return contacts;
        }

        public async Task<Contact> GetContact(string userName)
        {
            var contact = await _assessorDbContext.Contacts
                .FirstOrDefaultAsync(q => q.ContactName == userName && q.ContactStatus
                != ContactStatus.Deleted);
            if (contact == null)
                return null;

            var contactQueryViewModel = Mapper.Map<Contact>(contact);
            return contactQueryViewModel;
        }

        public async Task<bool> CheckContactExists(int contactId)
        {
            var result = await _assessorDbContext.Contacts
                         .AnyAsync(q => q.EndPointAssessorContactId == contactId && q.ContactStatus != ContactStatus.Deleted);
            return result;
        }

        public async Task<bool> CheckContactExists(Guid contactId)
        {
            var result = await _assessorDbContext.Contacts
                       .AnyAsync(q => q.Id == contactId && q.ContactStatus != ContactStatus.Deleted);
            return result;
        }

        public async Task<bool> CheckContactExists(string contactName, string contactEmail)
        {
            var result = await _assessorDbContext.Contacts
                     .AnyAsync(q => q.ContactName == contactName && q.ContactEmail == contactEmail && q.ContactStatus != ContactStatus.Deleted);
            return result;
        }
    }
}