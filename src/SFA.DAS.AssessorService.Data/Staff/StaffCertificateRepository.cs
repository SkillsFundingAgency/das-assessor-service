using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Staff
{
    public class StaffCertificateRepository : IStaffCertificateRepository
    {
        private readonly IDbConnection _connection;

        public StaffCertificateRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<CertificateForSearch>> GetCertificatesFor(long[] ulns)
        {
            return (await _connection.QueryAsync<CertificateForSearch>(@"SELECT cert.StandardCode, 
                                                                            cert.Uln, 
                                                                            cert.CertificateReference, 
		                                                                    cert.Status,
		                                                                    cert.UpdatedAt AS LastUpdatedAt
                                                                            FROM Certificates cert
                                                                            WHERE Uln IN @ulns",

                new {ulns})).ToList();
        }

        public async Task<List<CertificateLogSummary>> GetCertificateLogsFor(Guid certificateId,
            bool allRecords=false)
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
                var cert = await _connection.QueryFirstAsync<Certificate>("SELECT * FROM Certificates WHERE Id = @certificateId",
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
    }
}