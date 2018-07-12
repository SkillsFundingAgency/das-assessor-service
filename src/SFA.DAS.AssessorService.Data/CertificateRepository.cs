using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Data
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly AssessorDbContext _context;
        private readonly IDbConnection _connection;

        public CertificateRepository(AssessorDbContext context, IDbConnection connection)
        {
            _context = context;
            _connection = connection;
        }

        public async Task<Certificate> New(Certificate certificate)
        {
            await _context.Certificates.AddAsync(certificate);

            await UpdateCertificateLog(certificate, CertificateActions.Start, certificate.CreatedBy);

            await _context.SaveChangesAsync();

            return certificate;
        }

        public async Task<Certificate> GetCertificate(Guid id)
        {
            return await _context.Certificates.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Certificate> GetCertificate(long uln, int standardCode)
        {
            return await _context.Certificates.SingleOrDefaultAsync(c =>
                c.Uln == uln && c.StandardCode == standardCode);
        }

        public async Task<Certificate> GetCertificate(
            string certificateReference,
            string lastName,
            DateTime? achievementDate)
        {
            var certificate = await
                _context.Certificates
                    .FirstOrDefaultAsync(q => q.CertificateReference == certificateReference && CheckCertificateData(q, lastName, achievementDate));
            return certificate;
        }

        private bool CheckCertificateData(Certificate certificate, string lastName, DateTime? achievementDate)
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            return (certificateData.AchievementDate == achievementDate && certificateData.LearnerFamilyName == lastName);
        }

        public async Task<List<Certificate>> GetCompletedCertificatesFor(long uln)
        {
            return await _context.Certificates.Where(c =>
                    c.Uln == uln && (c.Status == CertificateStatus.Printed || c.Status == CertificateStatus.Submitted))
                .ToListAsync();
        }

        public async Task<List<Certificate>> GetCertificates(List<string> statuses)
        {
           if (statuses == null || !statuses.Any())
            {
                return await _context.Certificates
                    .Include(q => q.Organisation)
                    .ToListAsync();
            }
            else
           {
               return await _context.Certificates
                   .Include(q => q.Organisation)
                   .Where(x => statuses.Contains(x.Status))
                   .ToListAsync();
           }
        }

        public async Task<Certificate> Update(Certificate certificate, string username, string action)
        {
            var cert = await GetCertificate(certificate.Id);

            cert.CertificateData = certificate.CertificateData;
            cert.UpdatedBy = username;
            cert.Status = certificate.Status;
            cert.UpdatedBy = certificate.UpdatedBy;
            cert.UpdatedAt = certificate.UpdatedAt;

            await UpdateCertificateLog(cert, action, username);

            await _context.SaveChangesAsync();

            return cert;
        }

        private async Task UpdateCertificateLog(Certificate cert, string action, string username)
        {
            if (action != null)
            {
                var certLog = new CertificateLog
                {
                    Action = action,
                    CertificateId = cert.Id,
                    EventTime = DateTime.UtcNow,
                    Status = cert.Status,
                    Id = Guid.NewGuid(),
                    CertificateData = cert.CertificateData,
                    Username = username,
                    BatchNumber = cert.BatchNumber
                };

                await _context.CertificateLogs.AddAsync(certLog);
            }
        }

        public async Task UpdateStatuses(UpdateCertificatesBatchToIndicatePrintedRequest updateCertificatesBatchToIndicatePrintedRequest)
        {
            var toBePrintedDate = DateTime.UtcNow;

            foreach (var certificateStatus in updateCertificatesBatchToIndicatePrintedRequest.CertificateStatuses)
            {
                var certificate =
                    await _context.Certificates.FirstAsync(q => q.CertificateReference == certificateStatus.CertificateReference);

                certificate.BatchNumber = updateCertificatesBatchToIndicatePrintedRequest.BatchNumber;
                certificate.Status = CertificateStatus.Printed;
                certificate.ToBePrinted = toBePrintedDate;
                certificate.UpdatedBy = UpdatedBy.PrintFunctionFlow;

                await UpdateCertificateLog(certificate, CertificateActions.Printed, UpdatedBy.PrintFunctionFlow);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<CertificateLog>> GetCertificateLogsFor(Guid certificateId)
        {
            return await _context.CertificateLogs.Where(l => l.CertificateId == certificateId).OrderByDescending(l => l.EventTime).ToListAsync();
        }

        public Task<string> GetPreviousProviderName(int providerUkPrn)
        {
            return _connection.QueryFirstOrDefaultAsync<string>(@"SELECT TOP(1) JSON_VALUE(CertificateData, '$.ProviderName') 
                                                                  FROM Certificates 
                                                                  WHERE ProviderUkPrn = @providerUkPrn 
                                                                  AND JSON_VALUE(CertificateData, '$.ProviderName') IS NOT NULL 
                                                                  ORDER BY CreatedAt DESC", new {providerUkPrn});
        }
    }
}