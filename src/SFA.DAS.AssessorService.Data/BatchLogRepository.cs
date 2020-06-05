using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;

namespace SFA.DAS.AssessorService.Data
{
    public class BatchLogRepository : Repository, IBatchLogRepository
    {
        public BatchLogRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            SqlMapper.AddTypeHandler(typeof(BatchData), new BatchDataHandler());
        }

        public async Task<BatchLog> Create(BatchLog batchLog)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "INSERT INTO BatchLogs ([Period], [BatchCreated], [ScheduledDate], [BatchNumber], [NumberOfCertificates], [NumberOfCoverLetters], " +
                    "[CertificatesFileName], [FileUploadStartTime], [FileUploadEndTime], [BatchData])" +
                "VALUES (@period, @batchCreated, @scheduledDate, @batchNumber, @numberOfCertificates, @numberOfCoverLetters, " +
                    "@certificatesFileName, @fileUploadStartTime, @fileUploadEndTime, @batchData)",
                param: new
                {
                    batchLog.Period,
                    batchLog.BatchCreated,
                    batchLog.ScheduledDate,
                    batchLog.BatchNumber,
                    batchLog.NumberOfCertificates,
                    batchLog.NumberOfCoverLetters,
                    batchLog.CertificatesFileName,
                    batchLog.FileUploadStartTime,
                    batchLog.FileUploadEndTime,
                    batchLog.BatchData,
                },
                transaction: _unitOfWork.Transaction);

            return batchLog;
        }

        public async Task<ValidationResponse> UpdateBatchLogBatchWithDataRequest(Guid id, BatchData batchData)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE[BatchLogs] SET[BatchData] = @batchData WHERE[Id] = @id",
                param: new
                {
                    batchData,
                    id
                },
                transaction: _unitOfWork.Transaction);

            return new ValidationResponse();
        }
    }
}