using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Consts;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Paging;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Data
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly AssessorDbContext _context;
        private readonly IDbConnection _connection;
        private readonly ILogger<CertificateRepository> _logger;


        public CertificateRepository(AssessorDbContext context,
            IDbConnection connection)
        {
            _context = context;
            _connection = connection;
        }

        public async Task<Certificate> New(Certificate certificate)
        {
            // Another check closer to INSERT that there isn't already a cert for this uln / std code
            var existingCert = await _context.Certificates.FirstOrDefaultAsync(c =>
                c.Uln == certificate.Uln && c.StandardCode == certificate.StandardCode);
            if (existingCert == null)
            {
                _context.Certificates.Add(certificate);
                _context.SaveChanges();
                await UpdateCertificateLog(certificate, CertificateActions.Start, certificate.CreatedBy);
                _context.SaveChanges();

                return certificate;
            }

            return existingCert;
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

        public async Task<Certificate> GetPrivateCertificate(long uln, string lastName)
        {
            return await _context.Certificates.SingleOrDefaultAsync(c =>
                c.Uln == uln && c.IsPrivatelyFunded && CheckLastName(c.CertificateData, lastName));
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
            return await _context.Certificates.Where(c => c.Uln == uln && (c.Status == CertificateStatus.Printed || c.Status == CertificateStatus.Submitted))
                .ToListAsync();
        }

        public async Task<List<Certificate>> GetCertificates(List<string> statuses)
        {
            if (statuses == null || !statuses.Any())
            {
                return await _context.Certificates
                    .Include(q => q.Organisation)
                    .AsNoTracking()
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

        public async Task<PaginatedList<Certificate>> GetCertificateHistory(string userName, int pageIndex, int pageSize)
        {
            var statuses = new List<string>
            {
                CertificateStatus.Submitted,
                CertificateStatus.Printed,
                CertificateStatus.Reprint
            };

            var count = await (from certificate in _context.Certificates
                               join organisation in _context.Organisations on
                                 certificate.OrganisationId equals organisation.Id
                               join contact in _context.Contacts on
                                 organisation.Id equals contact.OrganisationId
                               where contact.Username == userName
                                  && statuses.Contains(certificate.Status)
                               select certificate).CountAsync();

            var ids = await (from certificate in _context.Certificates
                             join organisation in _context.Organisations on
                               certificate.OrganisationId equals organisation.Id
                             join contact in _context.Contacts on
                               organisation.Id equals contact.OrganisationId
                             join certificateLog in _context.CertificateLogs on
                                 certificate.Id equals certificateLog.CertificateId
                             where contact.Username == userName
                               && statuses.Contains(certificate.Status)
                             group certificate by new { certificate.Id, certificate.CreatedAt } into result
                             orderby result.Key.CreatedAt descending
                             select result.FirstOrDefault().Id)
                                        .Skip((pageIndex - 1) * pageSize)
                                        .Take(pageSize).ToListAsync();

            var certificates = await _context.Certificates.Where(q => ids.Contains(q.Id))
                .Include(q => q.Organisation)
                .Include(q => q.CertificateLogs)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return new PaginatedList<Certificate>(certificates, count, pageIndex, pageSize);
        }

        public async Task<Certificate> Update(Certificate certificate, string username, string action, bool updateLog = true)
        {
            var cert = await GetCertificate(certificate.Id);

            cert.CertificateData = certificate.CertificateData;
            cert.UpdatedBy = username;
            cert.Status = certificate.Status;
            cert.UpdatedAt = certificate.UpdatedAt;

            if (updateLog)
            {
                await UpdateCertificateLog(cert, action, username);
            }

            await _context.SaveChangesAsync();

            return cert;
        }

        public Task<Certificate> UpdateProviderName(Guid id, string providerName)
        {
            var certificate = GetCertificate(id).GetAwaiter().GetResult();

            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            certificateData.ProviderName = providerName;

            certificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _context.SaveChanges();

            return Task.FromResult(certificate);
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
                certificate.UpdatedBy = UpdatedBy.PrintFunction;

                await UpdateCertificateLog(certificate, CertificateActions.Printed, UpdatedBy.PrintFunction);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<CertificateLog>> GetCertificateLogsFor(Guid[] certificateIds)
        {
            return await _context.CertificateLogs.Where(l => certificateIds.Contains(l.CertificateId)).OrderByDescending(l => l.EventTime).ToListAsync();
        }
        public async Task<List<CertificateLog>> GetCertificateLogsFor(Guid certificateId)
        {
            return await _context.CertificateLogs.Where(l => l.CertificateId == certificateId).OrderByDescending(l => l.EventTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CertificateAddress> GetContactPreviousAddress(string userName)
        {
            var statuses = new List<string>
            {
                CertificateStatus.Submitted,
                CertificateStatus.Printed,
                CertificateStatus.Reprint
            };

            var certificateAddress = await (from certificateLog in _context.CertificateLogs
                                            join certificate in _context.Certificates on certificateLog.CertificateId equals certificate.Id
                                            where statuses.Contains(certificate.Status) && certificateLog.Username == userName
                                            let certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData)
                                            orderby certificate.UpdatedAt descending
                                            select new CertificateAddress
                                            {
                                                OrganisationId = certificate.OrganisationId,
                                                ContactOrganisation = certificateData.ContactOrganisation,
                                                ContactName = certificateData.ContactName,
                                                Department = certificateData.Department,
                                                CreatedAt = certificate.CreatedAt,
                                                AddressLine1 = certificateData.ContactAddLine1,
                                                AddressLine2 = certificateData.ContactAddLine2,
                                                AddressLine3 = certificateData.ContactAddLine3,
                                                City = certificateData.ContactAddLine4,
                                                PostCode = certificateData.ContactPostCode
                                            }).FirstOrDefaultAsync();

            return certificateAddress;
        }

        public Task<string> GetPreviousProviderName(int providerUkPrn)
        {
            return _connection.QueryFirstOrDefaultAsync<string>(@"SELECT TOP(1) JSON_VALUE(CertificateData, '$.ProviderName') 
                                                                  FROM Certificates 
                                                                  WHERE ProviderUkPrn = @providerUkPrn 
                                                                  AND JSON_VALUE(CertificateData, '$.ProviderName') IS NOT NULL 
                                                                  ORDER BY CreatedAt DESC", new { providerUkPrn });
        }

        private bool CheckLastName(string data, string lastName)
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(data);
            return certificateData.LearnerFamilyName == lastName;
        }
    }
}