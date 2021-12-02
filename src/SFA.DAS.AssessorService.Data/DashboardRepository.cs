using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class DashboardRepository : Repository, IDashboardRepository
    {
        public DashboardRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public async Task<EpaoDashboardResult> GetEpaoDashboard(string endPointAssessorOrganisationId, int pipelineCutoff)
        {
            var result = await _unitOfWork.Connection.QuerySingleAsync<EpaoDashboardResult>(
                "GetEPAO_DashboardCounts",
                new
                {
                    epaOrgId = endPointAssessorOrganisationId,
                    pipelineCutoff = pipelineCutoff
                },
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure);

            return result;
        }
    }
}
