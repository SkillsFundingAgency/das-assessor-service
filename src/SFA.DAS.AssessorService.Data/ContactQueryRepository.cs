namespace SFA.DAS.AssessorService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Application.Interfaces;
    using AutoMapper;
    using Domain.Consts;
    using Microsoft.EntityFrameworkCore;

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
                .SelectMany(q => q.Contacts).Where(q => q.Status == ContactStatus.Live)
                .Select(contact => Mapper.Map<Contact>(contact)).AsNoTracking().ToListAsync();

            return contacts;
        }

        public async Task<Contact> GetContact(string userName)
        {
            var contact = await _assessorDbContext.Contacts
                .FirstOrDefaultAsync(q => q.Username == userName && q.Status
                != ContactStatus.Deleted);
            if (contact == null)
                return null;

            var contactQueryViewModel = Mapper.Map<Contact>(contact);
            return contactQueryViewModel;
        }

        //public async Task<bool> CheckContactExists(int contactId)
        //{
        //    var result = await _assessorDbContext.Contacts
        //                 .AnyAsync(q => q.EndPointAssessorContactId == contactId && q.Status != Status.Deleted);
        //    return result;
        //}

        public async Task<bool> CheckContactExists(Guid contactId)
        {
            var result = await _assessorDbContext.Contacts
                       .AnyAsync(q => q.Id == contactId && q.Status != ContactStatus.Deleted);
            return result;
        }

        public async Task<bool> CheckContactExists(string userName)
        {
            var result = await _assessorDbContext.Contacts
                     .AnyAsync(q => q.Username == userName && q.Status != ContactStatus.Deleted);
            return result;
        }
    }
}