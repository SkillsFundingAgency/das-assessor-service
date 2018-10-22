using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Staff
{
    public class StaffCertificateRepository : IStaffCertificateRepository
    {
        private readonly AssessorDbContext _context;
        private readonly IDbConnection _connection;

        public StaffCertificateRepository(AssessorDbContext context, IDbConnection connection)
        {
            _context = context;
            _connection = connection;
        }

        public async Task<bool> IsPrivateCertificate(string certificateReference)
        {
            var certificate =
                await _context.Certificates
                    .Include(q => q.Organisation)
                    .FirstOrDefaultAsync(q => q.CertificateReference == certificateReference);
            return certificate != null && certificate.IsPrivatelyFunded;
        }

        public async Task<IEnumerable<Ilr>> SearchForLearnerByCertificateReference(string certRef)
        {
            var cert = await _context.Certificates.FirstOrDefaultAsync(c => c.CertificateReference == certRef);
            IEnumerable<Ilr> results =
                cert != null
                    ? new List<Ilr> {new Ilr().GetFromCertificate(cert)}
                    : new List<Ilr>();

            return results;
        }

        public async Task<List<CertificateForSearch>> GetCertificatesFor(long[] ulns)
        {
            return (await _connection.QueryAsync<CertificateForSearch>(@"SELECT 
                                                                            org.EndPointAssessorOrganisationId,
                                                                            cert.StandardCode, 
                                                                            cert.Uln, 
                                                                            cert.CertificateReference, 
                                                                            JSON_VALUE(CertificateData, '$.LearnerGivenNames') AS GivenNames, 
                                                                            JSON_VALUE(CertificateData, '$.LearnerFamilyName') AS FamilyName, 
		                                                                    cert.Status,
		                                                                    cert.UpdatedAt AS LastUpdatedAt
                                                                            FROM Certificates cert
																			INNER JOIN Organisations org
																			ON cert.OrganisationId = org.Id                                                                            
                                                                            WHERE Uln IN @ulns",
                new {ulns})).ToList();
        }

        public async Task<List<CertificateLogSummary>> GetCertificateLogsFor(Guid certificateId,
            bool allRecords = false)
        {
            if (allRecords)
            {
                return (await _connection.QueryAsync<CertificateLogSummary>(
                    @"SELECT EventTime, Action, ISNULL(c.DisplayName, logs.Username) AS ActionBy, logs.Status, logs.CertificateData, logs.BatchNumber 
                    FROM CertificateLogs logs
                    LEFT OUTER JOIN Contacts c ON c.Username = logs.Username
                    WHERE CertificateId = @certificateId
                    ORDER BY EventTime DESC", new {certificateId})).ToList();
            }
            else
            {
                var cert = await _connection.QueryFirstAsync<Certificate>(
                    "SELECT * FROM Certificates WHERE Id = @certificateId",
                    new {certificateId});

                if (cert.Status == CertificateStatus.Submitted
                    || cert.Status == CertificateStatus.Reprint
                    || cert.Status == CertificateStatus.Printed)
                {
                    return (await _connection.QueryAsync<CertificateLogSummary>(@"DECLARE @FirstSubmitTime datetime2

                                SELECT @FirstSubmitTime = MIN(EventTime) FROM CertificateLogs WHERE CertificateId = @certificateId AND Action = 'Submit' 

                                SELECT EventTime, Action, ISNULL(c.DisplayName, logs.Username) AS ActionBy, logs.Status, logs.CertificateData, logs.BatchNumber 
                                FROM CertificateLogs logs
                                LEFT OUTER JOIN Contacts c ON c.Username = logs.Username
                                WHERE CertificateId = @certificateId 
                                AND EventTime >= @FirstSubmitTime
                                ORDER BY EventTime DESC
                                ", new {certificateId})).ToList();
                }
                else
                {
                    return (await _connection.QueryAsync<CertificateLogSummary>(@"
                        SELECT TOP(1) EventTime, Action, ISNULL(c.DisplayName, logs.Username) AS ActionBy, logs.Status, logs.CertificateData, logs.BatchNumber
                        FROM CertificateLogs logs
                            LEFT OUTER JOIN Contacts c ON c.Username = logs.Username
                        WHERE CertificateId = @certificateId
                        ORDER BY EventTime DESC", new {certificateId})).ToList();
                }
            }
        }

        public async Task<StaffReposBatchSearchResult> GetCertificateLogsForBatch(int batchNumber, int page,
            int pageSize)
        {
            var results = await _context.CertificateLogs
                .Where(cl => cl.BatchNumber == batchNumber && cl.Action == CertificateActions.Printed)
                .Include(cl => cl.Certificate)
                .OrderByDescending(cl => cl.EventTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var count = await _context.CertificateLogs
                .Where(cl => cl.BatchNumber == batchNumber && cl.Action == CertificateActions.Printed).CountAsync();

            return new StaffReposBatchSearchResult {PageOfResults = results, TotalCount = count};
        }

        public async Task<StaffReposBatchLogResult> GetBatchLogs(int page, int pageSize)
        {
            var results = await _context.BatchLogs
                .OrderByDescending(q => q.BatchCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var count = await _context.BatchLogs.CountAsync();

            return new StaffReposBatchLogResult {PageOfResults = results, TotalCount = count};
        }
    }
}