using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class BatchLogRepository : IBatchLogRepository
    {
        private readonly IAssessorUnitOfWork _unitOfWork;
        public BatchLogRepository(IAssessorUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BatchLog> Create(BatchLog batchLog)
        {
            await _unitOfWork.AssessorDbContext.BatchLogs.AddAsync(batchLog);
            await _unitOfWork.AssessorDbContext.SaveChangesAsync();
            return batchLog;
        }

        public async Task<ValidationResponse> UpdateBatchLogSentToPrinter(BatchLog updatedBatchLog)
        {
            var batchLog = await _unitOfWork.AssessorDbContext.BatchLogs
                .FirstOrDefaultAsync(b => b.BatchNumber == updatedBatchLog.BatchNumber);

            var response = new ValidationResponse();
            if (batchLog == null)
            {
                response.Errors.Add(new ValidationErrorDetail("BatchNumber", $"Error: The batch log {updatedBatchLog.BatchNumber} does not exist."));
                return response;
            }

            batchLog.BatchCreated = updatedBatchLog.BatchCreated;
            batchLog.NumberOfCertificates = updatedBatchLog.NumberOfCertificates;
            batchLog.NumberOfCoverLetters = updatedBatchLog.NumberOfCoverLetters;
            batchLog.CertificatesFileName = updatedBatchLog.CertificatesFileName;
            batchLog.FileUploadStartTime = updatedBatchLog.FileUploadStartTime;
            batchLog.FileUploadEndTime = updatedBatchLog.FileUploadEndTime;
            batchLog.BatchData = updatedBatchLog.BatchData;

            await _unitOfWork.AssessorDbContext.SaveChangesAsync();

            return response;
        }

        public async Task<ValidationResponse> UpdateBatchLogPrinted(BatchLog updatedBatchLog)
        {
            var batchLog = await _unitOfWork.AssessorDbContext.BatchLogs
                .FirstOrDefaultAsync(b => b.BatchNumber == updatedBatchLog.BatchNumber);

            var response = new ValidationResponse();

            if (batchLog == null)
            {
                response.Errors.Add(new ValidationErrorDetail("BatchNumber", $"Error: The batch log {updatedBatchLog.BatchNumber} does not exist."));
                return response;
            }

            batchLog.BatchData = updatedBatchLog.BatchData;

            await _unitOfWork.AssessorDbContext.SaveChangesAsync();

            return response;
        }

        public async Task UpsertCertificatesReadyToPrintInBatch(Guid[] certificateIds, int batchNumber)
        {
            var certificateReferencesToUpdate = await _unitOfWork.AssessorDbContext.Set<CertificateBase>()
                .Where(c => certificateIds.Contains(c.Id))
                .Select(c => c.CertificateReference)
                .ToListAsync();

            var existingBatchLogs = await _unitOfWork.AssessorDbContext.CertificateBatchLogs
                .Where(cbl => certificateReferencesToUpdate.Contains(cbl.CertificateReference) && cbl.BatchNumber == batchNumber)
                .ToListAsync();

            var certificateReferencesInBatchLog = existingBatchLogs.Select(cbl => cbl.CertificateReference).ToList();

            if (certificateReferencesInBatchLog.Any())
            {
                var certificatesToUpdate = await _unitOfWork.AssessorDbContext.Set<CertificateBase>()
                    .Where(c => certificateReferencesInBatchLog.Contains(c.CertificateReference))
                    .ToListAsync();

                foreach (var batchLog in existingBatchLogs)
                {
                    var certificate = certificatesToUpdate.First(c => c.CertificateReference == batchLog.CertificateReference);
                    batchLog.CertificateData = certificate.CertificateData;
                    batchLog.Status = certificate.Status;
                    batchLog.StatusAt = certificate.UpdatedAt ?? certificate.CreatedAt;
                    batchLog.UpdatedAt = DateTime.UtcNow;
                    batchLog.UpdatedBy = SystemUsers.PrintFunction;
                }

                _unitOfWork.AssessorDbContext.CertificateBatchLogs.UpdateRange(existingBatchLogs);
                await _unitOfWork.AssessorDbContext.SaveChangesAsync();
            }

            var certificateReferencesToInsert = certificateReferencesToUpdate.Except(certificateReferencesInBatchLog).ToList();

            if (certificateReferencesToInsert.Any())
            {
                var certificatesToInsert = await _unitOfWork.AssessorDbContext.Set<CertificateBase>()
                    .Where(c => certificateReferencesToInsert.Contains(c.CertificateReference))
                    .ToListAsync();

                var newBatchLogs = certificatesToInsert.Select(c => new CertificateBatchLog
                {
                    Id = Guid.NewGuid(),
                    CertificateReference = c.CertificateReference,
                    BatchNumber = batchNumber,
                    CertificateData = c.CertificateData,
                    Status = c.Status,
                    StatusAt = c.UpdatedAt ?? c.CreatedAt,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = SystemUsers.PrintFunction
                }).ToList();

                await _unitOfWork.AssessorDbContext.CertificateBatchLogs.AddRangeAsync(newBatchLogs);
                await _unitOfWork.AssessorDbContext.SaveChangesAsync();
            }
        }
    }
}