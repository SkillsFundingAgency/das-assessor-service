using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IDbConnection _connection;

        public DashboardRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<EpaoDashboardResult> GetEpaoDashboard(string endPointAssessorOrganisationId)
        {
            var result = await _connection.QuerySingleAsync<EpaoDashboardResult>("GetEPAO_DashboardCounts", new
            {
                EPAOId = endPointAssessorOrganisationId
            }, commandType: CommandType.StoredProcedure);

            return result;
        }
    }
}
