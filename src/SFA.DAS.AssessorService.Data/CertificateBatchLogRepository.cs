using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class CertificateBatchLogRepository : Repository, ICertificateBatchLogRepository
    {
        private readonly AssessorDbContext _context;

        public CertificateBatchLogRepository(IUnitOfWork unitOfWork, AssessorDbContext context)
            : base(unitOfWork)
        {
            _context = context;
        }

        public async Task<CertificateBatchLog> GetCertificateBatchLog(string certificateReference, int batchNumber)
        {
            var certificateBatchLog = await
                _context.CertificateBatchLogs
                .FirstOrDefaultAsync(q => q.CertificateReference == certificateReference && q.BatchNumber == batchNumber);

            return certificateBatchLog;
        }

        public async Task UpsertCertificatesReadyToPrintInBatch(Guid[] certificateIds, int batchNumber)
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
