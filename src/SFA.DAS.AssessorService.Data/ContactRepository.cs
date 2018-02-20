namespace SFA.DAS.AssessorService.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class ContactRepository : IContactRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public ContactRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<IEnumerable<ContactQueryViewModel>> GetContacts(int ukprn)
        {
            var contacts = await _assessorDbContext.Contacts
                .Where(q => q.EndPointAssessorUKPRN == ukprn)
                .Select(q => Mapper.Map<ContactQueryViewModel>(q)).ToListAsync();

            return contacts;
        }


        public async Task<IEnumerable<ContactQueryViewModel>> GetContacts(string contactName)
        {
            var contacts = await _assessorDbContext.Contacts
                .Where(q => q.ContactName == contactName)
                .Select(q => Mapper.Map<ContactQueryViewModel>(q)).ToListAsync();

            return contacts;
        }

        public async Task<bool> CheckContactExists(int contactId)
        {
            var result = await _assessorDbContext.Contacts
                         .AnyAsync(q => q.EndPointAssessorContactId == contactId);
            return result;
        }
    }
}