using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace SFA.DAS.AssessorService.Data
{
    public class ApprovalsExtractRepository : Repository, IApprovalsExtractRepository
    {
        public ApprovalsExtractRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public async Task<DateTime?> GetLatestExtractTimestamp()
        {
                var latestDate = await _unitOfWork.Connection.QueryAsync<DateTime?>(
                    @"SELECT MAX(CASE WHEN ISNULL(CreatedOn, 0) > ISNULL(UpdatedOn, 0) THEN CreatedOn ELSE UpdatedOn END) AS MaxDate FROM ApprovalsExtract",
                    transaction: _unitOfWork.Transaction);

                return latestDate.FirstOrDefault();
            
        }

        public void UpsertApprovalsExtract(List<ApprovalsExtract> approvalsExtract)
        {
            try
            {
                DapperPlusManager.Entity<ApprovalsExtract>().Table("ApprovalsExtract");
                _unitOfWork.Begin();
                _unitOfWork.Transaction.BulkMerge(approvalsExtract);
                _unitOfWork.Commit();
            }
            catch(Exception ex)
            {
                _unitOfWork.Rollback();
                throw ex;
            }
        }
    }
}
