using System;
using System.Collections.Generic;
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
                "INSERT INTO BatchLogs ([Period], [ScheduledDate], [BatchNumber])" +
                "VALUES (@period, @scheduledDate, @batchNumber)",
                param: new
                {
                    batchLog.Period,
                    batchLog.ScheduledDate,
                    batchLog.BatchNumber
                },
                transaction: _unitOfWork.Transaction);

            return batchLog;
        }
        public async Task<ValidationResponse> UpdateBatchLogSentToPrinter(BatchLog updatedBatchLog)
        {
            var rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [BatchLogs] SET " +
                "   [BatchCreated] = @batchCreated, " +
                "   [NumberOfCertificates] = @numberOfCertificates, " +
                "   [NumberOfCoverLetters] = @numberOfCoverLetters, " +
                "   [CertificatesFileName] = @certificatesFileName, " +
                "   [FileUploadStartTime] = @fileUploadStartTime, " +
                "   [FileUploadEndTime] = @fileUploadEndTime, " +
                "   [BatchData] = @batchData " +
                "WHERE " +
                "   [BatchNumber] = @batchNumber",
                param: new
                {
                    updatedBatchLog.BatchNumber,
                    updatedBatchLog.BatchCreated,
                    updatedBatchLog.NumberOfCertificates,
                    updatedBatchLog.NumberOfCoverLetters,
                    updatedBatchLog.CertificatesFileName,
                    updatedBatchLog.FileUploadStartTime,
                    updatedBatchLog.FileUploadEndTime,
                    updatedBatchLog.BatchData
                },
                transaction: _unitOfWork.Transaction);

            var response = new ValidationResponse();
            if(rowsAffected == 0)
            {
                response.Errors.Add(new ValidationErrorDetail("BatchNumber", $"Error the batch log {updatedBatchLog.BatchNumber} does not exist."));
            }
            
            return response;
        }

        public async Task<ValidationResponse> UpdateBatchLogPrinted(BatchLog updatedBatchLog)
        {
            var rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [BatchLogs] SET " +
                "   [BatchData] = @batchData " +
                "WHERE " +
                "   [BatchNumber] = @batchNumber",
                param: new
                {
                    updatedBatchLog.BatchNumber,
                    updatedBatchLog.BatchData
                },
                transaction: _unitOfWork.Transaction);

            var response = new ValidationResponse();
            if (rowsAffected == 0)
            {
                response.Errors.Add(new ValidationErrorDetail("BatchNumber", $"Error the batch log {updatedBatchLog.BatchNumber} does not exist."));
            }

            return response;
        }
    }
}