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
                @"SELECT MAX(LastUpdated) AS MaxDate FROM ApprovalsExtract",
                transaction: _unitOfWork.Transaction);

            return latestDate.FirstOrDefault();
        }

        public async Task UpsertApprovalsExtractToStaging(List<ApprovalsExtract> approvalsExtract)
        {
            if (null == approvalsExtract || !approvalsExtract.Any()) return;

            try
            {
                if (_unitOfWork.Connection.State != ConnectionState.Open)
                    _unitOfWork.Connection.Open();

                await BulkInsertApprovalsExtractStaging(approvalsExtract);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_unitOfWork.Connection.State != ConnectionState.Closed)
                    _unitOfWork.Connection.Close();
            }
        }

        /// <summary>
        /// Copy All records retrieved from Approvals into the staging table
        /// </summary>
        /// <param name="approvalsExtract"></param>
        /// <returns></returns>
        private async Task BulkInsertApprovalsExtractStaging(IEnumerable<ApprovalsExtract> approvalsExtract)
        {
            var dataTable = ConstructApprovalsExtractDataTable(approvalsExtract);
            using (var bulkCopy = new SqlBulkCopy(_unitOfWork.Connection as SqlConnection))
            {
                bulkCopy.DestinationTableName = "ApprovalsExtract_Staging";
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

        public async Task ClearApprovalsExtractStaging()
        {
            await _unitOfWork.Connection.ExecuteAsync("DELETE FROM [dbo].ApprovalsExtract_Staging", commandTimeout: 0);
        }

        /// <summary>
        /// Execute the Insert/Update Stored Procedure then clear down the table for staging for future batches.
        /// </summary>
        /// <returns></returns>
        public async Task PopulateApprovalsExtract()
        {
            try
            {
                var result = await _unitOfWork.Connection.ExecuteScalarAsync<int>("ImportIntoApprovalsExtract_FromApprovalsExtract_Staging", transaction: _unitOfWork.Transaction, commandType: CommandType.StoredProcedure, commandTimeout: 300);

                if (result != 0)
                {
                    throw new Exception("Stored procedure ImportIntoApprovalsExtract_FromApprovalsExtract_Staging failed to complete successfully.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> PopulateLearner()
        {
            try
            {
                var upsertedCount = await _unitOfWork.Connection.ExecuteScalarAsync<int>("PopulateLearner", commandType: CommandType.StoredProcedure, commandTimeout: 0);
                return upsertedCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
