using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Data
{
    public class OrganisationQueryRepository : IOrganisationQueryRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public OrganisationQueryRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<IEnumerable<Organisation>> GetAllOrganisations()
        {
            return await _assessorDbContext.Organisations.ToListAsync();
        }

        public async Task<Organisation> GetByUkPrn(long ukprn)
        {
            return await _assessorDbContext.Organisations
                .FirstOrDefaultAsync(q => q.EndPointAssessorUkprn == ukprn);
        }
        

        public async Task<Organisation> Get(string endPointAssessorOrganisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                .FirstAsync(q =>
                    q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId);

            //var organisationUpdateDomainModel = Mapper.Map<OrganisationDomainModel>(organisation);
            return organisation;
        }

        public async Task<Organisation> Get(Guid id)
        {
            return await _assessorDbContext.Organisations.SingleAsync(o => o.Id == id);
        }

        public async Task<bool> CheckIfAlreadyExists(string endPointAssessorOrganisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                .FirstOrDefaultAsync(q =>
                    q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId);
            return organisation != null;
        }

        public async Task<bool> CheckIfAlreadyExists(Guid id)
        {
            var organisation = await _assessorDbContext.Organisations
                .FirstOrDefaultAsync(q => q.Id == id);
            return organisation != null;
        }

        public async Task<bool> CheckIfOrganisationHasContacts(string endPointAssessorOrganisationId)
        {
            var organisation = await _assessorDbContext.Organisations
                .Include(q => q.Contacts)
                .FirstOrDefaultAsync(q =>
                    q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId);
            return organisation.Contacts.Count() != 0;
        }

        public async Task<bool> CheckIfOrganisationHasContactsWithSigninId(string endPointAssessorOrganisationId, Guid contactId)
        {
            var organisation = await _assessorDbContext.Organisations
                .Include(q => q.Contacts)
                .FirstOrDefaultAsync(q =>
                    q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId);
             //Ignore calling contact
             return organisation.Contacts?.Any(x => x.SignInId != null && x.Id != contactId) ?? false;
        }

        public async Task<Organisation> GetOrganisationByName(string name)
        {

            return await _assessorDbContext.Organisations.Include(x => x.OrganisationType).
                FirstOrDefaultAsync(x => x.OrganisationData.LegalName == name);
        }

        public async Task<Organisation> GetOrganisationByContactId(Guid contactId)
        {
            var contact = await _assessorDbContext
                .Contacts
                .Include(c => c.Organisation)
                .Include(c => c.Organisation.OrganisationType)
                .FirstOrDefaultAsync(c => c.Id == contactId);
            
            return contact
                .Organisation;
        }
    }
}