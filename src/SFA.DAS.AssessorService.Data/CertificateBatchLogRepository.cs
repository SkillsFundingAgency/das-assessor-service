using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
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

        public async Task<List<CertificatePrintSummary>> GetCertificatesForBatch(int batchNumber)
        {
            var sql =
               @"SELECT 
                    c.[Uln], 
                    c.[StandardCode], 
                    c.[ProviderUkPrn], 
                    c.[CertificateReference], 
                    c.[BatchNumber], 
                    c.[Status], 
                    o.[EndPointAssessorOrganisationId], 
                    o.[EndPointAssessorName], 
                    c.[CertificateData] 
                FROM 
                    [CertificateBatchLogs] cbl INNER JOIN [Certificates] c 
                       ON cbl.CertificateReference = c.CertificateReference INNER JOIN [Organisations] o 
                       ON c.OrganisationId = o.Id 
                WHERE 
                    cbl.BatchNumber = @batchNumber";

            var certificates = await _unitOfWork.Connection.QueryAsync<Certificate, Organisation, CertificateData, CertificatePrintSummary>(
                sql, (certificate, organisation, certificateData) =>
                {
                    var certificatePrintSummary = new CertificatePrintSummary()
                    {
                        Uln = certificate.Uln,
                        StandardCode = certificate.StandardCode,
                        ProviderUkPrn = certificate.ProviderUkPrn,
                        EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                        EndPointAssessorOrganisationName = organisation.EndPointAssessorName,
                        CertificateReference = certificate.CertificateReference,
                        BatchNumber = certificate.BatchNumber.GetValueOrDefault().ToString(),
                        LearnerGivenNames = certificateData.LearnerGivenNames,
                        LearnerFamilyName = certificateData.LearnerFamilyName,
                        StandardName = certificateData.StandardName,
                        StandardLevel = certificateData.StandardLevel,
                        ContactName = certificateData.ContactName,
                        ContactOrganisation = certificateData.ContactOrganisation,
                        ContactAddLine1 = certificateData.ContactAddLine1,
                        ContactAddLine2 = certificateData.ContactAddLine2,
                        ContactAddLine3 = certificateData.ContactAddLine3,
                        ContactAddLine4 = certificateData.ContactAddLine4,
                        ContactPostCode = certificateData.ContactPostCode,
                        AchievementDate = certificateData.AchievementDate,
                        CourseOption = certificateData.CourseOption,
                        OverallGrade = certificateData.OverallGrade,
                        Department = certificateData.Department,
                        FullName = certificateData.FullName,
                        Status = certificate.Status
                    };
                    return certificatePrintSummary;
                },
                splitOn: "Uln, EndPointAssessorOrganisationId, CertificateData",
                param: new { batchNumber },
                transaction: _unitOfWork.Transaction);

            return certificates.ToList();
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
