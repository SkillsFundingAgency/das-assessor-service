using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;

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

        public async Task<List<CertificateLogSummary>> GetCertificateLogsFor(Guid certificateId)
        {
            return (await _connection.QueryAsync<CertificateLogSummary>(
                @"SELECT EventTime, Action, c.DisplayName AS ActionBy, logs.Status, logs.CertificateData, logs.BatchNumber 
                    FROM CertificateLogs logs
                    INNER JOIN Contacts c ON c.Username = logs.Username
                    WHERE CertificateId = @certificateId
                    ORDER BY EventTime DESC", new {certificateId})).ToList();
        }
    }
}