using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Paging;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Data
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly AssessorDbContext _context;
        private readonly IDbConnection _connection;
        
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
                c.Uln == certificate.Uln && c.StandardCode == certificate.StandardCode && c.CreateDay == certificate.CreateDay);
            if (existingCert != null) return existingCert;
            
            await _context.Certificates.AddAsync(certificate);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (!(e.InnerException is SqlException sqlException)) throw;

                if (sqlException.Number == 2601 || sqlException.Number == 2627)
                {
                    return await _context.Certificates.FirstOrDefaultAsync(c =>
                        c.Uln == certificate.Uln && c.StandardCode == certificate.StandardCode && c.CreateDay == certificate.CreateDay);
                }
                throw;
            }
                
            await UpdateCertificateLog(certificate, CertificateActions.Start, certificate.CreatedBy);
            await _context.SaveChangesAsync();

            return certificate;
        }

        public async Task<Certificate> NewPrivate(Certificate certificate,
            string endpointOrganisationId)
        {
            // Another check closer to INSERT that there isn't already a cert for this uln / std code
            var existingCert = await _context.Certificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c =>
                    c.Uln == certificate.Uln &&
                    c.Organisation.EndPointAssessorOrganisationId == endpointOrganisationId &&
                    c.IsPrivatelyFunded);

            if (existingCert != null)
                return existingCert;

            await _context.Certificates.AddAsync(certificate);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (!(e.InnerException is SqlException sqlException)) throw;

                if (sqlException.Number == 2601 || sqlException.Number == 2627)
                {
                    return await _context.Certificates.FirstOrDefaultAsync(c =>
                        c.Uln == certificate.Uln && c.StandardCode == certificate.StandardCode &&
                        c.CreateDay == certificate.CreateDay);
                }

                throw;
            }

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

        public async Task<Certificate> GetPrivateCertificate(long uln,
            string endpointOrganisationId)
        {
            var existingCert = await _context.Certificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c =>
                    c.Uln == uln &&
                    c.Organisation.EndPointAssessorOrganisationId == endpointOrganisationId);
            return existingCert;
        }

        public async Task<Certificate> GetCertificateByOrgIdLastname(long uln,
            string endpointOrganisationId, string lastName)
        {
            var existingCert = await _context.Certificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c =>
                    c.Uln == uln &&
                    c.Organisation.EndPointAssessorOrganisationId == endpointOrganisationId &&
                    CheckCertificateData(c, lastName));
            return existingCert;
        }

        public async Task<Certificate> GetCertificateByUlnLastname(long uln,
            string lastName)
        {
            var existingCert = await _context.Certificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c =>
                    c.Uln == uln &&
                    CheckCertificateData(c, lastName));

            return existingCert;
        }


        public async Task<bool> CertifciateExistsForUln(long uln)
        {
            var existingCert = await _context.Certificates
                .AnyAsync(c =>
                    c.Uln == uln);

            return existingCert;
        }

        public async Task<Certificate> GetCertificateDeletedByUln(long uln)
        {
            var existingCert = await _context.Certificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c =>
                    c.Uln == uln &&
                    c.Status == CertificateStatus.Deleted);

            return existingCert;
        }

        private bool CheckCertificateData(Certificate certificate, string lastName)
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            return (certificateData.LearnerFamilyName == lastName);
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
            return await _context.Certificates.Where(c => c.Uln == uln && (c.Status == CertificateStatus.Reprint || c.Status == CertificateStatus.Printed || c.Status == CertificateStatus.Submitted || c.Status == CertificateStatus.ToBeApproved))
                .ToListAsync();
        }

        public async Task<List<Certificate>> GetCertificates(List<string> statuses)
        {
            if (statuses == null || !statuses.Any())
            {
                return await _context.Certificates
                    .Include(q => q.Organisation)
                    .Include(q => q.CertificateLogs)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else 
            {
                return await _context.Certificates
                    .Include(q => q.Organisation)
                    .Include(q => q.CertificateLogs)
                    .Where(x => statuses.Contains(x.Status))
                    .ToListAsync();
            }
        }

        public async Task<PaginatedList<Certificate>> GetCertificatesForApproval(int pageIndex, int pageSize,string status, string privatelyFundedStatus)
        {
            int count;
            var certificates = _context.Certificates.Include(q => q.Organisation)
                .Include(q => q.CertificateLogs);
            IQueryable<Certificate> queryable;
            if (status == null )
            {
                count = await _context.Certificates.Where(x => x.IsPrivatelyFunded).CountAsync();
                queryable = certificates;
            }
            else
            {
               count = await _context.Certificates.Where(x => x.Status == status && x.PrivatelyFundedStatus == privatelyFundedStatus).Where(x => x.IsPrivatelyFunded).CountAsync();
               queryable = certificates.Where(x => x.Status == status && x.PrivatelyFundedStatus == privatelyFundedStatus);
            }

            if (pageSize == 0)
                  pageSize = count == 0 ? 1 : count;
            var  certificateResult = await queryable
                .Where(x => x.IsPrivatelyFunded) 
                .OrderByDescending(q => q.UpdatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            return new PaginatedList<Certificate>(certificateResult, count, pageIndex < 0 ? 1 : pageIndex, pageSize);
        }


        public async Task<PaginatedList<Certificate>> GetCertificateHistory(string endPointAssessorOrganisationId, int pageIndex, int pageSize, List<string> statuses)
        {
          
            var count = await GetCertificatesCount(endPointAssessorOrganisationId, statuses);

            var ids = await (from certificate in _context.Certificates
                             join organisation in _context.Organisations on
                               certificate.OrganisationId equals organisation.Id
                             where organisation.EndPointAssessorOrganisationId == endPointAssessorOrganisationId
                               && !statuses.Contains(certificate.Status)
                             orderby certificate.CreatedAt descending
                             select certificate.Id)
                                        .Skip((pageIndex - 1) * pageSize)
                                        .Take(pageSize).ToListAsync();

            var certificates = await _context.Certificates.Where(q => ids.Contains(q.Id))
                .Include(q => q.Organisation)
                .Include(q => q.CertificateLogs)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return new PaginatedList<Certificate>(certificates, count, pageIndex, pageSize);
        }

        public async Task<int> GetCertificatesCount(string endPointAssessorOrganisationId, List<string> statuses)
        {
           
            return  await (from certificate in _context.Certificates
                join organisation in _context.Organisations on
                    certificate.OrganisationId equals organisation.Id
                where organisation.EndPointAssessorOrganisationId == endPointAssessorOrganisationId
                      && !statuses.Contains(certificate.Status)
                select certificate).CountAsync();
        }

        public async Task<Certificate> Update(Certificate certificate, string username, string action, bool updateLog = true, string reasonForChange = null)
        {
            var cert = await GetCertificate(certificate.Id);

            cert.Uln = certificate.Uln;
            cert.CertificateData = certificate.CertificateData;
            cert.Status = certificate.Status;
            cert.ProviderUkPrn = certificate.ProviderUkPrn;
            cert.StandardCode = certificate.StandardCode;
            cert.UpdatedBy = username;
            cert.UpdatedAt = DateTime.UtcNow;

            if (cert.IsPrivatelyFunded)
            {
                cert.PrivatelyFundedStatus = certificate.PrivatelyFundedStatus;
            }

            if (certificate.Status != CertificateStatus.Deleted)
            {
                cert.DeletedBy =  null;
                cert.DeletedAt = null;
            }

            if (updateLog)
            {
                await UpdateCertificateLog(cert, action, username, reasonForChange);
            }
            
            await _context.SaveChangesAsync();

            return cert;
        }

        public async Task Delete(long uln, int standardCode, string username, string action, bool updateLog = true)
        {
            var cert = await GetCertificate(uln, standardCode);

            if (cert == null) throw new NotFound();

            // If already deleted ignore
            if (cert.Status == CertificateStatus.Deleted)
                return;

            cert.Status = CertificateStatus.Deleted;
            cert.DeletedBy = username;
            cert.DeletedAt = DateTime.UtcNow;

            if (updateLog)
            {
                await UpdateCertificateLog(cert, action, username);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Certificate> UpdateProviderName(Guid id, string providerName)
        {
            var certificate = await GetCertificate(id);

            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            certificateData.ProviderName = providerName;

            certificate.CertificateData = JsonConvert.SerializeObject(certificateData);           
            await _context.SaveChangesAsync();

            return certificate;
        }

        private async Task UpdateCertificateLog(Certificate cert, string action, string username, string reasonForChange = null)
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
                    BatchNumber = cert.BatchNumber,
                    ReasonForChange = reasonForChange
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
        

        public async Task<CertificateAddress> GetContactPreviousAddress(string userName, bool isPrivatelyFunded)
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
                                                            && certificate.IsPrivatelyFunded == isPrivatelyFunded
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
                                                                  ORDER BY CreatedAt DESC", new {providerUkPrn});
        }

        public async Task UpdatePrivatelyFundedCertificatesToBeApproved()
        {
            var certificates =
                _context.Certificates.Where(q => q.IsPrivatelyFunded && q.Status == CertificateStatus.Submitted &&
                                                 q.PrivatelyFundedStatus != CertificateStatus.Approved);
            if (certificates.Any())
            {
                foreach (var certificate in certificates)
                {
                    certificate.Status = CertificateStatus.ToBeApproved;
                    certificate.PrivatelyFundedStatus = null;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task ApproveCertificates(List<ApprovalResult> approvalResults, string userName)
        {
            var certificateReferences =
                approvalResults.Select(q => q.CertificateReference).ToList();

            var certificates =
                _context.Certificates.Where(q => certificateReferences.Contains(q.CertificateReference));
            if (certificates.Any())
            {
                foreach (var approvalResult in approvalResults)
                {
                    var certificate =
                        await certificates.FirstOrDefaultAsync(
                            q => q.CertificateReference == approvalResult.CertificateReference
                                 && q.PrivatelyFundedStatus != CertificateStatus.Approved);

                    if (certificate == null) continue;

                    if (approvalResult.IsApproved == CertificateStatus.ToBeApproved &&
                        approvalResult.PrivatelyFundedStatus == CertificateStatus.SentForApproval)
                    {
                        var certLog = await _context.CertificateLogs.FirstOrDefaultAsync(x =>
                            x.CertificateId == certificate.Id && x.Action == CertificateStatus.SentForApproval &&
                            x.Status == CertificateStatus.ToBeApproved);
                        if (certLog != null) {
                            var deleted =  _context.CertificateLogs.Any(x => x.CertificateId == certificate.Id && x.Status == CertificateStatus.Deleted);
                            if (!deleted)
                                continue;
                        }
                    }

                    certificate.Status = approvalResult.IsApproved;
                    certificate.PrivatelyFundedStatus = approvalResult.PrivatelyFundedStatus;

                    await UpdateCertificateLog(certificate, approvalResult.PrivatelyFundedStatus, userName,
                        approvalResult.ReasonForChange);

                }


                await _context.SaveChangesAsync();
            }
        }

        private bool CheckLastName(string data, string lastName)
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(data);
            return certificateData.LearnerFamilyName == lastName;
        }

        public async Task<List<Option>> GetOptions(int stdCode)
        {
            return (await _connection.QueryAsync<Option>("SELECT * FROM Options WHERE StdCode = @stdCode",
                new {stdCode})).ToList();
        }

        private bool CheckLastNameExists(Certificate certificate, Certificate c)
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            var certificateDataCompare = JsonConvert.DeserializeObject<CertificateData>(c.CertificateData);
            return certificateData.LearnerFamilyName == certificateDataCompare.LearnerFamilyName;
        }
    }
}