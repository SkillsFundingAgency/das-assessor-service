using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class BatchLogQueryRepository : Repository, IBatchLogQueryRepository
    {
        public BatchLogQueryRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            SqlMapper.AddTypeHandler(typeof(BatchData), new BatchDataHandler());
            SqlMapper.AddTypeHandler(typeof(CertificateData), new CertificateDataHandler());
        }

        public async Task<BatchLog> Get(int batchNumber)
        {
            var batchLog = await _unitOfWork.Connection.QueryAsync<BatchLog>(
                @"SELECT TOP 1 
                    [Id], 
                    [Period], 
                    [BatchCreated], 
                    [ScheduledDate], 
                    [BatchNumber], 
                    [NumberOfCertificates], 
                    [NumberOfCoverLetters], 
                    [CertificatesFileName], 
                    [FileUploadStartTime], 
                    [FileUploadEndTime], 
                    [BatchData] 
                  FROM BatchLogs WHERE BatchNumber = @batchNumber",
            param: new { batchNumber},
                transaction: _unitOfWork.Transaction);

            return batchLog.FirstOrDefault();
        }

        public async Task<CertificateBatchLog> GetCertificateBatchLog(int batchNumber, string certificateReference)
        {
            var certificateBatchlog = await _unitOfWork.Connection.QueryAsync<CertificateBatchLog>(
                @"SELECT TOP 1 
                    [Id], 
                    [CertificateReference], 
                    [BatchNumber], 
                    [CertificateData], 
                    [Status], 
                    [StatusAt], 
                    [ReasonForChange], 
                    [CreatedAt], 
                    [CreatedBy], 
                    [UpdatedAt], 
                    [UpdatedBy], 
                    [DeletedAt], 
                    [DeletedBy] 
                FROM 
                    CertificateBatchLogs 
                WHERE 
                    BatchNumber = @batchNumber AND CertificateReference = @certificateReference",
            param: new { batchNumber, certificateReference },
                transaction: _unitOfWork.Transaction);

            return certificateBatchlog.FirstOrDefault();
        }

        public async Task<BatchLog> GetLastBatchLog()
        {
            var results = await _unitOfWork.Connection.QueryAsync<BatchLog>(
                @"SELECT TOP 1 
                    [Period], 
                    [BatchCreated], 
                    [ScheduledDate], 
                    [BatchNumber], 
                    [NumberOfCertificates], 
                    [NumberOfCoverLetters], 
                    [CertificatesFileName], 
                    [FileUploadStartTime], 
                    [FileUploadEndTime], 
                    [BatchData] 
                  FROM BatchLogs ORDER BY BatchCreated DESC",
                transaction: _unitOfWork.Transaction);

            var batchLog = results.FirstOrDefault();

            if (batchLog == null)
            {
                batchLog = new BatchLog
                {
                    BatchNumber = 0
                };
                return batchLog;
            }

            return batchLog;
        }

        public async Task<int?> GetBatchNumberReadyToPrint()
        {
            var batchNumber = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<int?>(
                @"SELECT TOP 1 
                    [BatchNumber] 
                  FROM 
                    BatchLogs 
                  WHERE 
                    FileUploadEndTime IS NULL 
                  ORDER BY 
                    BatchNumber",
                transaction: _unitOfWork.Transaction);

            return batchNumber;
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
    }
}