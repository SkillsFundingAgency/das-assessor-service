using Dapper;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                @"INSERT INTO BatchLogs ([Period], [ScheduledDate], [BatchNumber]) 
                  VALUES (@period, @scheduledDate, @batchNumber)",
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
                @"UPDATE [BatchLogs] SET 
                    [BatchCreated] = @batchCreated, 
                    [NumberOfCertificates] = @numberOfCertificates, 
                    [NumberOfCoverLetters] = @numberOfCoverLetters, 
                    [CertificatesFileName] = @certificatesFileName, 
                    [FileUploadStartTime] = @fileUploadStartTime, 
                    [FileUploadEndTime] = @fileUploadEndTime, 
                    [BatchData] = @batchData 
                  WHERE 
                    [BatchNumber] = @batchNumber",
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
            if (rowsAffected == 0)
            {
                response.Errors.Add(new ValidationErrorDetail("BatchNumber", $"Error the batch log {updatedBatchLog.BatchNumber} does not exist."));
            }

            return response;
        }

        public async Task<ValidationResponse> UpdateBatchLogPrinted(BatchLog updatedBatchLog)
        {
            var rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                @"UPDATE [BatchLogs] SET 
                    [BatchData] = @batchData 
                  WHERE 
                    [BatchNumber] = @batchNumber",
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

        public async Task UpsertCertificatesReadyToPrintInBatch(int batchNumber, Guid[] certificateIds)
        {
            var certificateIdsToUpdate = await _unitOfWork.Connection.QueryAsync<Guid>(
                @"SELECT 
                    c.[Id] 
                  FROM 
                    [Certificates] c
                  INNER JOIN 
                    [CertificateBatchLogs] cbl 
                    ON cbl.CertificateReference = c.CertificateReference 
                  WHERE 
                    c.Id IN @certificateIds 
                    AND cbl.BatchNumber = @batchNumber",
                param: new { certificateIds, batchNumber },
                transaction: _unitOfWork.Transaction);

            if (certificateIdsToUpdate.Count() > 0)
            {
                await _unitOfWork.Connection.ExecuteAsync(
                    @"UPDATE [CertificateBatchLogs] 
                      SET 
                          CertificateData = c.CertificateData, 
                          Status = c.Status, 
                          StatusAt = ISNULL(c.UpdatedAt, c.CreatedAt), 
                          UpdatedAt = GETUTCDATE(), 
                          UpdatedBy = @updatedBy 
                      FROM 
                          [Certificates] c
                      INNER JOIN 
                          [CertificateBatchLogs] cbl 
                          ON cbl.CertificateReference = c.CertificateReference
                      WHERE 
                          c.Id IN @certificateIdsToUpdate
                          AND cbl.BatchNumber = @batchNumber",
                    param: new { certificateIdsToUpdate, batchNumber, updatedBy = SystemUsers.PrintFunction },
                    transaction: _unitOfWork.Transaction);
            }

            var certificateIdsToInsert = certificateIds.Except(certificateIdsToUpdate);
            if (certificateIdsToInsert.Count() > 0)
            {
                await _unitOfWork.Connection.ExecuteAsync(
                    @"INSERT INTO [CertificateBatchLogs] 
                      ( 
                          CertificateReference, 
                          BatchNumber, 
                          CertificateData, 
                          Status, 
                          StatusAt, 
                          CreatedAt, 
                          CreatedBy 
                      ) 
                      SELECT 
                        c.CertificateReference, 
                        @batchNumber, 
                        c.CertificateData, 
                        c.Status, 
                        ISNULL(c.UpdatedAt, c.CreatedAt) StatusAt, 
                        GETUTCDATE(), 
                        @createdBy 
                      FROM 
                        [Certificates] c 
                      WHERE 
                        c.Id IN @certificateIdsToInsert",
                    param: new { certificateIdsToInsert, batchNumber, createdBy = SystemUsers.PrintFunction },
                    transaction: _unitOfWork.Transaction);
            }
        }
    }
}