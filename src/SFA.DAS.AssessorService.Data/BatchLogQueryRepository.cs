using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class BatchLogQueryRepository : IBatchLogQueryRepository
    {
        private readonly IAssessorUnitOfWork _unitOfWork;

        public BatchLogQueryRepository(IAssessorUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BatchLog> Get(int batchNumber)
        {
            return await _unitOfWork.AssessorDbContext.BatchLogs
                .Where(bl => bl.BatchNumber == batchNumber)
                .OrderBy(bl => bl.Id) 
                .FirstOrDefaultAsync();
        }

        public async Task<CertificateBatchLog> GetCertificateBatchLog(int batchNumber, string certificateReference)
        {
            return await _unitOfWork.AssessorDbContext.CertificateBatchLogs
                .Where(cbl => cbl.BatchNumber == batchNumber && cbl.CertificateReference == certificateReference)
                .FirstOrDefaultAsync();
        }

        public async Task<BatchLog> GetLastBatchLog()
        {
            var batchLog = await _unitOfWork.AssessorDbContext.BatchLogs
                .OrderByDescending(bl => bl.BatchCreated)
                .FirstOrDefaultAsync();

            return batchLog ?? new BatchLog { BatchNumber = 0 };
        }

        public async Task<int?> GetBatchNumberReadyToPrint()
        {
            return await _unitOfWork.AssessorDbContext.BatchLogs
                .Where(bl => bl.FileUploadEndTime == null)
                .OrderBy(bl => bl.BatchNumber)
                .Select(bl => (int?)bl.BatchNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CertificatePrintSummaryBase>> GetCertificatesForBatch(int batchNumber)
        {
            var standardCertificates = await _unitOfWork.AssessorDbContext.StandardCertificates
                .Include(sc => sc.Organisation)
                .Include(sc => sc.CertificateBatchLog)
                .Where(sc => sc.CertificateBatchLog.BatchNumber == batchNumber)
                .ToListAsync();

            var frameworkCertificates = await _unitOfWork.AssessorDbContext.FrameworkCertificates
                .Include(fc => fc.CertificateBatchLog)
                .Where(fc => fc.CertificateBatchLog.BatchNumber == batchNumber)
                .ToListAsync();

            var standardSummaries = standardCertificates.Select(sc => new CertificatePrintSummary
            {
                Uln = sc.Uln,
                StandardCode = sc.StandardCode,
                ProviderUkPrn = sc.ProviderUkPrn,
                EndPointAssessorOrganisationId = sc.Organisation?.EndPointAssessorOrganisationId,
                EndPointAssessorOrganisationName = sc.Organisation?.EndPointAssessorName,
                CertificateReference = sc.CertificateReference,
                BatchNumber = sc.BatchNumber?.ToString(),
                LearnerGivenNames = sc.CertificateData.LearnerGivenNames,
                LearnerFamilyName = sc.CertificateData.LearnerFamilyName,
                StandardName = sc.CertificateData.StandardName,
                StandardLevel = sc.CertificateData.StandardLevel,
                CoronationEmblem = sc.CertificateData.CoronationEmblem,
                ContactName = sc.CertificateData.ContactName,
                ContactOrganisation = sc.CertificateData.ContactOrganisation,
                ContactAddLine1 = sc.CertificateData.ContactAddLine1,
                ContactAddLine2 = sc.CertificateData.ContactAddLine2,
                ContactAddLine3 = sc.CertificateData.ContactAddLine3,
                ContactAddLine4 = sc.CertificateData.ContactAddLine4,
                ContactPostCode = sc.CertificateData.ContactPostCode,
                AchievementDate = sc.CertificateData.AchievementDate,
                CourseOption = sc.CertificateData.CourseOption,
                OverallGrade = sc.CertificateData.OverallGrade,
                Department = sc.CertificateData.Department,
                FullName = sc.CertificateData.FullName,
                Status = sc.Status
            }).ToList();

            var frameworkSummaries = frameworkCertificates.Select(fc => new FrameworkCertificatePrintSummary
            {
                CertificateReference = fc.CertificateReference,
                BatchNumber = fc.BatchNumber?.ToString(),
                LearnerGivenNames = fc.CertificateData.LearnerGivenNames,
                LearnerFamilyName = fc.CertificateData.LearnerFamilyName,
                ContactName = fc.CertificateData.ContactName,
                ContactAddLine1 = fc.CertificateData.ContactAddLine1,
                ContactAddLine2 = fc.CertificateData.ContactAddLine2,
                ContactAddLine3 = fc.CertificateData.ContactAddLine3,
                ContactAddLine4 = fc.CertificateData.ContactAddLine4,
                ContactPostCode = fc.CertificateData.ContactPostCode,
                AchievementDate = fc.CertificateData.AchievementDate,
                FullName = fc.CertificateData.FullName,
                Status = fc.Status
            }).ToList();

            return standardSummaries.Cast<CertificatePrintSummaryBase>()
                .Concat(frameworkSummaries.Cast<CertificatePrintSummaryBase>())
                .ToList();
        }
    }
}