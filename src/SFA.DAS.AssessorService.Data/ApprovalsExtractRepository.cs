using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task UpsertApprovalsExtract(List<ApprovalsExtract> approvalsExtract)
        {
            if (null == approvalsExtract || !approvalsExtract.Any()) return;

            try
            {
                // Incoming batch should be no more than 1000 ids
                var uniqueIncomingIds = approvalsExtract.Select(ae => ae.ApprenticeshipId).Distinct();

                _unitOfWork.Begin();
                var query = "SELECT ApprenticeshipId FROM ApprovalsExtract WHERE ApprenticeshipId in @ids";
                var existingIds = await _unitOfWork.Connection.QueryAsync<int>(
                    query, uniqueIncomingIds.ToArray(), _unitOfWork.Transaction);

                var newIds = uniqueIncomingIds.Except(existingIds);

                var approvalsExtractToInsert = approvalsExtract.Where(ae => newIds.Contains(ae.ApprenticeshipId));
                await BulkInsertApprovalsExtract(approvalsExtractToInsert);

                var approvalsExtractToUpdate = approvalsExtract.Where(ae => existingIds.Contains(ae.ApprenticeshipId));
                await UpdateApprovalsExtract(approvalsExtractToUpdate);
                                
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw ex;
            }
        }

        private async Task BulkInsertApprovalsExtract(IEnumerable<ApprovalsExtract> approvalsExtract)
        {
            var bulkCopyOptions = SqlBulkCopyOptions.Default;
            var dataTable = ConstructApprovalsExtractDataTable(approvalsExtract);

            using (var bulkCopy = new SqlBulkCopy(_unitOfWork.Connection as SqlConnection, bulkCopyOptions, _unitOfWork.Transaction as SqlTransaction))
            {
                bulkCopy.DestinationTableName = "ApprovalsExtract";
                await bulkCopy.WriteToServerAsync(dataTable);
            }
        }

        private DataTable ConstructApprovalsExtractDataTable(IEnumerable<ApprovalsExtract> approvalsExtract)
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("ApprenticeshipId");
            dataTable.Columns.Add("FirstName");
            dataTable.Columns.Add("LastName");
            dataTable.Columns.Add("ULN");
            dataTable.Columns.Add("TrainingCode");
            dataTable.Columns.Add("TrainingCourseVersion");
            dataTable.Columns.Add("TrainingCourseVersionConfirmed");
            dataTable.Columns.Add("TrainingCourseOption");
            dataTable.Columns.Add("StandardUId");
            dataTable.Columns.Add("StartDate");
            dataTable.Columns.Add("EndDate");
            dataTable.Columns.Add("CreatedOn");
            dataTable.Columns.Add("UpdatedOn");
            dataTable.Columns.Add("StopDate");
            dataTable.Columns.Add("PauseDate");
            dataTable.Columns.Add("CompletionDate");
            dataTable.Columns.Add("UKPRN");
            dataTable.Columns.Add("LearnRefNumber");
            dataTable.Columns.Add("PaymentStatus");

            foreach (var ae in approvalsExtract)
            {
                dataTable.Rows.Add(ae.ApprenticeshipId, ae.FirstName, ae.LastName, ae.ULN, ae.TrainingCode, ae.TrainingCourseVersion, ae.TrainingCourseVersionConfirmed,
                    ae.TrainingCourseOption, ae.StandardUId, ae.StartDate, ae.EndDate, ae.CreatedOn, ae.UpdatedOn, ae.StopDate,
                    ae.PauseDate, ae.CompletionDate, ae.UKPRN, ae.LearnRefNumber, ae.PaymentStatus);
            }

            return dataTable;
        }


        private async Task UpdateApprovalsExtract(IEnumerable<ApprovalsExtract> approvalsExtract)
        {
            var updateSQL = "UPDATE ApprovalsExtract SET FirstName = @FirstName, LastName = @LastName, Uln = @Uln, TrainingCode = @TrainingCode, TrainingCourseVersion = @TrainingCourseVersion, TrainingCourseVersionConfirmed = @TrainingCourseVersionConfirmed, TrainingCourseOption = @TrainingCourseOption, StandardUid = @StandardUid, StartDate = @StartDate, EndDate = @EndDate, CreatedOn = @CreatedOn, UpdatedOn = @UpdatedOn, StopDate = @StopDate, PauseDate = @PauseDate, CompletionDate = @CompletionDate, UKPRN = @UKPRN, LearnRefNumber = @LearnRefNumber, PaymentStatus = @PaymentStatus WHERE ApprenticeshipId = @ApprenticeshipId;";
            await _unitOfWork.Connection.ExecuteAsync(updateSQL, approvalsExtract, _unitOfWork.Transaction);
        }

        public async Task<int> PopulateLearner()
        {
            try
            {
                var result = await _unitOfWork.Connection.ExecuteScalarAsync<int>("PopulateLearner", commandType: CommandType.StoredProcedure, commandTimeout: 300);
                if (0 != result)
                {
                    throw new Exception("Stored procedure PopulateLearner failed to complete successfully.");
                }
                int rowCount = await _unitOfWork.Connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Learner");
                return rowCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
