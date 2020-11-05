using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
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

        public async Task UpdateCertificatesReadyToPrintInBatch(Guid[] certificateIds, int batchNumber)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "INSERT INTO [CertificateBatchLogs] " +
                "(" +
                    "CertificateReference, " +
                    "BatchNumber, " +
                    "CertificateData, " +
                    "Status, " +
                    "StatusAt, " +
                    "CreatedAt, " +
                    "CreatedBy" +
                ") " +
                "SELECT " +
                    "c.CertificateReference, " +
                    "@batchNumber, " +
                    "c.CertificateData, " +
                    "c.Status, " +
                    "ISNULL(c.UpdatedAt, c.CreatedAt) StatusAt, " +
                    "GETUTCDATE(), " +
                    "@createdBy " +
                "FROM " +
                    "[Certificates] c " +
                "WHERE " +
                    "c.Id IN @certificateIds",
                param: new { certificateIds, batchNumber, createdBy = SystemUsers.PrintFunction },
                transaction: _unitOfWork.Transaction);
        }
    }
}
