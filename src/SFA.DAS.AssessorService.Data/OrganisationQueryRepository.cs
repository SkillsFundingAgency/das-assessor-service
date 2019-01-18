using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
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
        
    }
}