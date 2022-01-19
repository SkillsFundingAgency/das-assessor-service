using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Extensions;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
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
            return await _assessorDbContext.Organisations
                .Include(o => o.OrganisationType)
                .FirstOrDefaultAsync(q => q.EndPointAssessorOrganisationId == endPointAssessorOrganisationId);
        }

        public async Task<Organisation> Get(Guid id)
        {
            return await _assessorDbContext.Organisations
                .Include(o => o.OrganisationType)
                .SingleOrDefaultAsync(o => o.Id == id);
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

        public async Task<IEnumerable<Organisation>> GetOrganisationsByStandard(int standard)
        {
            var organisations = await _assessorDbContext
                .OrganisationStandard
                .Include(c => c.Organisation)
                .Include(c=>c.OrganisationStandardDeliveryAreas)
                .ThenInclude(c=>c.DeliveryArea)
                .Where(c => c.StandardCode == standard)
                .ToListAsync();

            return organisations.Select(c=>c.Organisation);
        }

        public async Task<Organisation> GetOrganisationByName(string name)
        {

            return await _assessorDbContext.Organisations
                .Include(x => x.OrganisationType)
                .FirstOrDefaultAsync(x => x.OrganisationData.LegalName == name);
        }

        public async Task<Organisation> GetOrganisationByContactId(Guid contactId)
        {
            var contact = await _assessorDbContext
                .Contacts
                .Include(c => c.Organisation)
                .Include(c => c.Organisation.OrganisationType)
                .FirstOrDefaultAsync(c => c.Id == contactId);
            
            return contact.Organisation;
        }

        public async Task<PaginatedList<MergeLogEntry>> GetOrganisationMergeLogs(int pageSize, int pageIndex, string orderBy, string orderDirection, string primaryEPAOId, string secondaryEPAOId)
        {
            IQueryable<MergeOrganisation> queryable = _assessorDbContext.MergeOrganisations;
            if(!string.IsNullOrWhiteSpace(primaryEPAOId))
            {
                queryable = queryable.Where(mo => mo.PrimaryEndPointAssessorOrganisationId == primaryEPAOId);
            }
            if (!string.IsNullOrWhiteSpace(secondaryEPAOId))
            {
                queryable = queryable.Where(mo => mo.SecondaryEndPointAssessorOrganisationId == secondaryEPAOId);
            }
            if(!string.IsNullOrWhiteSpace(orderBy))
            {
                if(!string.IsNullOrWhiteSpace(orderDirection) && orderDirection.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                {
                    queryable = queryable.OrderByDescending(orderBy);
                }
                else
                {
                    queryable = queryable.OrderBy(orderBy);
                }
            }
            else
            {
                queryable = queryable.OrderBy("CompletedAt");
            }
            int count = await queryable.CountAsync();

            if (pageSize == 0)
                pageSize = count == 0 ? 1 : count;

            var result = await queryable
                //.OrderByDescending(q => q.UpdatedAt)
                .Skip(((pageIndex > 0) ? pageIndex - 1 : 0) * pageSize)
                .Take(pageSize)
                .Select(o => new MergeLogEntry() 
                {
                    Id = o.Id,
                    PrimaryEndPointAssessorOrganisationId = o.PrimaryEndPointAssessorOrganisationId,
                    PrimaryEndPointAssessorOrganisationName = o.PrimaryEndPointAssessorOrganisationName,
                    SecondaryEndPointAssessorOrganisationId = o.SecondaryEndPointAssessorOrganisationId,
                    SecondaryEndPointAssessorOrganisationName = o.SecondaryEndPointAssessorOrganisationName,
                    CompletedAt = o.CompletedAt,
                    SecondaryEPAOEffectiveTo = o.SecondaryEPAOEffectiveTo
                })
                .ToListAsync();

            return new PaginatedList<MergeLogEntry>(result, count, pageIndex < 0 ? 1 : pageIndex, pageSize);
        }

        public async Task<MergeLogEntry> GetOrganisationMergeLogById(int id)
        {
            var o = await _assessorDbContext.MergeOrganisations.FirstOrDefaultAsync(mo => mo.Id == id);
            if (null == o) return null;
            return new MergeLogEntry()
            {
                Id = o.Id,
                PrimaryEndPointAssessorOrganisationId = o.PrimaryEndPointAssessorOrganisationId,
                PrimaryEndPointAssessorOrganisationName = o.PrimaryEndPointAssessorOrganisationName,
                SecondaryEndPointAssessorOrganisationId = o.SecondaryEndPointAssessorOrganisationId,
                SecondaryEndPointAssessorOrganisationName = o.SecondaryEndPointAssessorOrganisationName,
                CompletedAt = o.CompletedAt,
                SecondaryEPAOEffectiveTo = o.SecondaryEPAOEffectiveTo
            };
        }
    }
}