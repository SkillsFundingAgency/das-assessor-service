using Dapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Infrastructure;
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
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly ILogger<ApprovalsExtractRepository> _logger;

        public ApprovalsExtractRepository(IUnitOfWork unitOfWork, IRoatpApiClient roatpApiClient, ILogger<ApprovalsExtractRepository> logger)
            : base(unitOfWork)
        {
            _roatpApiClient = roatpApiClient;
            _logger = logger;
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
            dataTable.Columns.Add("EmployerAccountId");
            dataTable.Columns.Add("EmployerName");

            foreach (var ae in approvalsExtract)
            {
                long uln = 0;

                if (!long.TryParse(ae.ULN, out uln))
                {
                    _logger.LogWarning($"Invalid Uln {ae.ULN}");
                    continue;
                }

                dataTable.Rows.Add(ae.ApprenticeshipId, ae.FirstName, ae.LastName, uln, ae.TrainingCode, ae.TrainingCourseVersion, ae.TrainingCourseVersionConfirmed,
                    ae.TrainingCourseOption, ae.StandardUId, ae.StartDate, ae.EndDate, ae.CreatedOn, ae.UpdatedOn, ae.StopDate,
                    ae.PauseDate, ae.CompletionDate, ae.UKPRN, ae.LearnRefNumber, ae.PaymentStatus, ae.EmployerAccountId, ae.EmployerName);
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
                var result = await _unitOfWork.Connection.ExecuteScalarAsync<int>("ImportIntoApprovalsExtract_FromApprovalsExtract_Staging", transaction: _unitOfWork.Transaction, commandType: CommandType.StoredProcedure, commandTimeout: 0);

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

        public async Task InsertProvidersFromApprovalsExtract()
        {
            var missingUkprns = await UkprnsInExtractNotInProviders();
            await RefreshProviders(missingUkprns);
        }

        public async Task RefreshProviders()
        {
            var allUkprns = await UkprnsInProviders();
            await RefreshProviders(allUkprns);
        }

        private async Task RefreshProviders(IEnumerable<int> ukprns)
        {
            // Get the provider name from RoATP and update Providers table.

            foreach (var ukprn in ukprns)
            {
                var name = await GetProviderName(ukprn);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var existingName = await _unitOfWork.Connection.ExecuteScalarAsync<string>("SELECT Name FROM Providers WHERE Ukprn = @Ukprn;", new { Ukprn = ukprn });
                    if (string.IsNullOrWhiteSpace(existingName))
                    {
                        await _unitOfWork.Connection.ExecuteAsync("INSERT INTO Providers (Ukprn, Name, UpdatedOn) VALUES (@Ukprn, @Name, @UpdatedOn)", new { Ukprn = ukprn, Name = name, UpdatedOn = DateTime.UtcNow });
                    }
                    else
                    {
                        if (name != existingName)
                        {
                            await _unitOfWork.Connection.ExecuteAsync("UPDATE Providers SET Name = @Name, UpdatedOn = @UpdatedOn WHERE Ukprn = @Ukprn", new { Ukprn = ukprn, Name = name, UpdatedOn = DateTime.UtcNow });
                        }
                    }
                }
            }
        }

        private async Task<IEnumerable<int>> UkprnsInExtractNotInProviders()
        {
            var ukprns = await _unitOfWork.Connection.QueryAsync<int>("SELECT DISTINCT ax1.Ukprn FROM ApprovalsExtract ax1 LEFT JOIN Providers pr1 ON pr1.ukprn = ax1.ukprn WHERE pr1.ukprn IS NULL");
            return ukprns;
        }

        private async Task<IEnumerable<int>> UkprnsInProviders()
        {
            var ukprns = await _unitOfWork.Connection.QueryAsync<int>("SELECT DISTINCT Ukprn FROM Providers");
            return ukprns;
        }

        private async Task<string> GetProviderName(int ukprn)
        {
            var provider = (await _roatpApiClient.SearchOrganisationByUkprn(ukprn)).FirstOrDefault();
            if (provider == null)
            {
                _logger.LogError($"Training provider {ukprn} not found in RoATP.");
            }

            return provider?.ProviderName;
        }
    }
}
