using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Data
{
    public class OrganisationQueryRepository : IOrganisationQueryRepository
    {
        private readonly AssessorDbContext _assessorDbContext;
        private readonly IDbConnection _connection;

        public OrganisationQueryRepository(AssessorDbContext assessorDbContext, IDbConnection connection)
        {
            _assessorDbContext = assessorDbContext;
            _connection = connection;
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

        public async Task<int> GetEpaOrganisationStandardsCount(string endPointAssessorOrganisationId)
        {
            
            var epaoId = new SqlParameter("@EPAOId", endPointAssessorOrganisationId);
            var count = new SqlParameter("@Count", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            
            await _assessorDbContext.Database.ExecuteSqlCommandAsync("EXEC EPAO_Standards_Count @EPAOId, @Count out",  epaoId, count);
            return (int)count.Value;
        }

        public async Task<int> GetEpaoPipelineCount(string endPointAssessorOrganisationId)
        {
            var result = await _connection.QueryAsync<EPAOPipeline>("GetEPAO_Pipelines", new {
                EPAOId= endPointAssessorOrganisationId,
                SKIP = 0,
                TAKE = 1
            },
                commandType: CommandType.StoredProcedure);

            var epaoPipelines = result?.ToList();
            if (epaoPipelines != null && epaoPipelines.Any())
            {
                return epaoPipelines.Select(x => x.TotalRows).First();
            }

            return 0;
        }

    }
}