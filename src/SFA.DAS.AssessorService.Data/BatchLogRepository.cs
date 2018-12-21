using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Data
{
    public class BatchLogRepository : IBatchLogRepository
    {
        private readonly AssessorDbContext _assessorDbContext;
        private readonly IWebConfiguration _configuration;

        public BatchLogRepository(AssessorDbContext assessorDbContext, IWebConfiguration configuration)
        {
            _assessorDbContext = assessorDbContext;
            _configuration = configuration;
            SqlMapper.AddTypeHandler(typeof(BatchData), new BatchDataHandler());
        }

        public async Task<BatchLog> Create(BatchLog batchLog)
        {
            await _assessorDbContext.BatchLogs.AddAsync(batchLog);
            _assessorDbContext.SaveChanges();

            return batchLog;
        }

        public async Task<BatchLogResponse> GetBatchLogFromPeriodAndBatchNumber(string period, string batchNumber)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlForMainDetails =
                    "select * " +
                    " FROM BatchLogs " +
                    "WHERE Period = @period and BatchNumber = @batchNumber";
                var orgs = await connection.QueryAsync<BatchLogResponse>(sqlForMainDetails, new { period, batchNumber });
                var org = orgs.FirstOrDefault();
                return org;
            }
        }

        public async Task<ValidationResponse> UpdateBatchLogBatchWithDataRequest(Guid id, string batchData)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                   connection.Execute(
                    "UPDATE [BatchLogs] SET [BatchData] = @batchData WHERE [Id] = @id",
                    new { batchData, id });

                
            }

            return new ValidationResponse();
        }
    }
}