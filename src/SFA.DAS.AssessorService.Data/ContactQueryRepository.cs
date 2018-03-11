using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Data
{
    public class ContactQueryRepository : IContactQueryRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public ContactQueryRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<IEnumerable<ContactResponse>> GetContacts(string endPointAssessorOrganisationId)
        {
            var contacts = await _assessorDbContext.Organisations
                .Include(organisation => organisation.Contacts)
                .Where(organisation => organisation.EndPointAssessorOrganisationId == endPointAssessorOrganisationId
                                       && organisation.Status != OrganisationStatus.Deleted)
                .SelectMany(q => q.Contacts)
                .Select(contact => Mapper.Map<ContactResponse>(contact)).AsNoTracking().ToListAsync();

            return contacts;
        }

        public async Task<ContactResponse> GetContact(string userName)
        {
            var contact = await _assessorDbContext.Organisations
                .Include(organisation => organisation.Contacts)     
                .Where(q => q.Status != OrganisationStatus.Deleted)
                .SelectMany(q => q.Contacts)
                .Where(q => q.Username == userName)
                .Select(q => Mapper.Map<ContactResponse>(q)).AsNoTracking()
                .FirstOrDefaultAsync();          

            var contactRessponse = Mapper.Map<ContactResponse>(contact);
            return contactRessponse;
        }

        public async Task<bool> CheckContactExists(string userName)
        {
            var result = await _assessorDbContext.Contacts
                .AnyAsync(q => q.Username == userName);
            return result;
        }
    }
}