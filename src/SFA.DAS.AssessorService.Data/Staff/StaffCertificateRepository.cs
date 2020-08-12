using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.Staff
{
    public class StaffCertificateRepository : Repository, IStaffCertificateRepository
    {
        public StaffCertificateRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        { 
        }

        public async Task<List<CertificateForSearch>> GetCertificatesFor(long[] ulns)
        {
            var sql = @"
                SELECT 
                    org.EndPointAssessorOrganisationId,
                    cert.StandardCode,
                    JSON_VALUE(CertificateData, '$.StandardName') AS StandardName, 
                    cert.Uln, 
                    cert.CertificateReference, 
                    JSON_VALUE(CertificateData, '$.LearnerGivenNames') AS GivenNames, 
                    JSON_VALUE(CertificateData, '$.LearnerFamilyName') AS FamilyName, 
		            cert.Status,
		            cert.UpdatedAt AS LastUpdatedAt
                FROM 
                    Certificates cert INNER JOIN Organisations org
                    ON cert.OrganisationId = org.Id
                WHERE Uln IN @ulns";

            var results = await _unitOfWork.Connection.QueryAsync<CertificateForSearch>(
                sql,
                param: new { ulns },
                transaction: _unitOfWork.Transaction);

            return results.ToList();
        }

        public async Task<List<CertificateLogSummary>> GetAllCertificateLogs(Guid certificateId)
        {
            var sql = @"
                SELECT 
                    EventTime, 
                    Action, 
                    ISNULL(c.DisplayName, logs.Username) AS ActionBy, 
                    ISNULL(c.Email, '') AS ActionByEmail, 
                    logs.Status, 
                    logs.CertificateData, 
                    logs.BatchNumber, 
                    logs.ReasonForChange  
                FROM 
                    CertificateLogs logs LEFT OUTER JOIN Contacts c 
                    ON c.Username = logs.Username
                WHERE 
                    CertificateId = @certificateId
                ORDER BY EventTime DESC";

            var results = await _unitOfWork.Connection.QueryAsync<CertificateLogSummary>(
                    sql,
                    param: new { certificateId },
                    transaction: _unitOfWork.Transaction);

            return results.ToList();
        }

        public async Task<List<CertificateLogSummary>> GetSummaryCertificateLogs(Guid certificateId)
        {
            var sql = @"
                DECLARE @FirstSubmitTime datetime2

                SELECT 
                    @FirstSubmitTime = MIN(EventTime) 
                FROM 
                    CertificateLogs 
                WHERE 
                    CertificateId = @certificateId 
                    AND Action = 'Submit' 

                SELECT 
                    EventTime, 
                    Action, 
                    ISNULL(c.DisplayName, logs.Username) AS ActionBy, 
                    ISNULL(c.Email, '') AS ActionByEmail, 
                    logs.Status, 
                    logs.CertificateData, 
                    logs.BatchNumber, 
                    logs.ReasonForChange  
                FROM 
                    CertificateLogs logs LEFT OUTER JOIN Contacts c 
                    ON c.Username = logs.Username
                WHERE 
                    CertificateId = @certificateId 
                    AND EventTime >= @FirstSubmitTime
                    ORDER BY EventTime DESC";

            var results = await _unitOfWork.Connection.QueryAsync<CertificateLogSummary>(
                    sql,
                    param: new { certificateId },
                    transaction: _unitOfWork.Transaction);

            return results.ToList();
        }

        public async Task<CertificateLogSummary> GetLatestCertificateLog(Guid certificateId)
        {
            var sql = @"
                SELECT TOP(1) 
                    EventTime, 
                    Action, 
                    ISNULL(c.DisplayName, logs.Username) AS ActionBy, 
                    ISNULL(c.Email, '') AS ActionByEmail, 
                    logs.Status, 
                    logs.CertificateData, 
                    logs.BatchNumber, 
                    logs.ReasonForChange 
                FROM 
                    CertificateLogs logs LEFT OUTER JOIN Contacts c 
                    ON c.Username = logs.Username
                WHERE 
                    CertificateId = @certificateId
                ORDER BY 
                    EventTime DESC";

            var result = await _unitOfWork.Connection.QueryFirstAsync<CertificateLogSummary>(
                sql,
                param: new { certificateId },
                transaction: _unitOfWork.Transaction);

            return result;
        }

        public async Task<GetCertificateLogsForBatchResult> GetCertificateLogsForBatch(int batchNumber, int page, int pageSize)
        {
            var results = new GetCertificateLogsForBatchResult();

            var sql = @"
                SELECT
					b.FileUploadEndTime SentToPrinterAt,
					CAST(JSON_VALUE(BatchData, '$.PrintedDate') AS DATETIME2) PrintedAt
                FROM 
                    [BatchLogs] b
                WHERE
                    b.BatchNumber = @batchNumber;

                SELECT
                    cbl.BatchNumber,
                    cbl.StatusAt StatusAt,
                    cbl.Status,
                    cbl.CertificateData,
                    cbl.CertificateReference,
                    c.Uln,
                    c.StandardCode
                FROM 
                    [CertificateBatchLogs] cbl INNER JOIN [Certificates] c
                    ON cbl.CertificateReference = c.CertificateReference
                WHERE
                    cbl.BatchNumber = @batchNumber AND cbl.Status IN @printProcessStatus
                ORDER BY
                    c.CertificateReference
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT
                    COUNT(*)
                FROM 
                    [CertificateBatchLogs] cbl INNER JOIN [Certificates] c
                    ON cbl.CertificateReference = c.CertificateReference
                WHERE
                    cbl.BatchNumber = @batchNumber AND cbl.Status IN @printProcessStatus;";

            using (var multi = await _unitOfWork.Connection.QueryMultipleAsync(
                sql,
                param: new
                {
                    BatchNumber = batchNumber,
                    CertificateStatus.PrintProcessStatus,
                    OffSet = (page-1) * pageSize,
                    PageSize = pageSize
                },
                transaction: _unitOfWork.Transaction))
            {
                var printResult = multi.ReadFirst();
                results.SentToPrinterAt = printResult.SentToPrinterAt;
                results.PrintedAt = printResult.PrintedAt;

                results.PageOfResults = multi.Read<CertificateBatchLogSummary>().ToList();
                results.TotalCount = multi.ReadFirst<int>(); 
            }

            return results;
        }

        public async Task<GetBatchLogsResult> GetBatchLogs(int page, int pageSize)
        {
            var result = new GetBatchLogsResult();

            var sql = @"
                SELECT
					BatchNumber,
					ScheduledDate,
					FileUploadEndTime SentToPrinterAt,
					NumberOfCertificates NumberOfCertificatesSent,
					CAST(JSON_VALUE(BatchData, '$.PrintedDate') AS DATETIME2) PrintedAt,
					JSON_VALUE(BatchData, '$.TotalCertificateCount') NumberOfCertificatesPrinted
				FROM 
                    [BatchLogs]
                ORDER BY
                    FileUploadEndTime DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT
                    COUNT(*)
                FROM 
                    [BatchLogs];";

            using (var multi = await _unitOfWork.Connection.QueryMultipleAsync(
                sql,
                param: new
                {
                    OffSet = (page - 1) * pageSize,
                    PageSize = pageSize
                },
                transaction: _unitOfWork.Transaction))
            {
                result.PageOfResults = multi.Read<BatchLogSummary>().ToList();
                result.TotalCount = multi.ReadFirst<int>();
            }

            return result;
        }
    }
}