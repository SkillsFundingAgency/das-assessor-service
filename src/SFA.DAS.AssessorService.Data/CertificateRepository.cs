using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;
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

            await AddCertificateLog(certificate.Id, CertificateActions.Start, certificate.Status, DateTime.UtcNow, 
                certificate.CertificateData, certificate.CreatedBy, certificate.BatchNumber);
            
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

            await AddCertificateLog(certificate.Id, CertificateActions.Start, certificate.Status, DateTime.UtcNow,
                certificate.CertificateData, certificate.CreatedBy, certificate.BatchNumber);

            await _context.SaveChangesAsync();

            return certificate;
        }

        public async Task<Certificate> GetCertificate(Guid id)
        {
            return await _context.Certificates.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Certificate> GetCertificate(long uln, int standardCode)
        {
            return await _context.Certificates
                .Include(q => q.CertificateBatchLog)
                .SingleOrDefaultAsync(c =>
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

        public async Task<Certificate> GetCertificate(string certificateReference)
        {
            var certificate = await
               _context.Certificates
                    .FirstOrDefaultAsync(q => q.CertificateReference == certificateReference);
            return certificate;
        }

        private bool CheckCertificateData(Certificate certificate, string lastName, DateTime? achievementDate)
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            return (certificateData.AchievementDate == achievementDate && certificateData.LearnerFamilyName == lastName);
        }

        public async Task<List<Certificate>> GetCompletedCertificatesFor(long uln)
        {
            var completedCertificateStatus = new[] { CertificateStatus.Submitted, CertificateStatus.ToBeApproved }.Concat(CertificateStatus.PrintProcessStatus).ToList();
            return await _context.Certificates.Where(c => c.Uln == uln && completedCertificateStatus.Contains(c.Status))
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

        public async Task<List<CertificateToBePrintedSummary>> GetCertificatesToBePrinted(List<string> statuses)
        {
            var certificatesToBePrinted = 
                await(
                    from certificate in _context.Certificates
                    join organisation in _context.Organisations on certificate.OrganisationId equals organisation.Id
                    where statuses.Contains(certificate.Status)
                    let certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData)
                    select new CertificateToBePrintedSummary
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
                    }).ToListAsync();

            return certificatesToBePrinted;
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
                .Include(q => q.CertificateBatchLog)
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

            cert.BatchNumber = null;

            if (updateLog)
            {
                await AddCertificateLog(cert.Id, action, cert.Status, cert.UpdatedAt.Value,
                    cert.CertificateData, cert.UpdatedBy, cert.BatchNumber, reasonForChange);
            }
            
            await _context.SaveChangesAsync();

            return cert;
        }

        public async Task Delete(long uln, int standardCode, string username, string action, bool updateLog = true, string reasonForChange = null, string incidentNumber = null)
        {
            var certificate = await GetCertificate(uln, standardCode);

            if (certificate == null) throw new NotFound();

            // If already deleted ignore
            if (certificate.Status == CertificateStatus.Deleted)
                return;

            certificate.Status = CertificateStatus.Deleted;
            certificate.DeletedBy = username;
            certificate.DeletedAt = DateTime.UtcNow;

            if (incidentNumber != null)
            {
                UpdateIncidentNumber(incidentNumber, certificate);
            }

            if (updateLog)
            {
                await AddCertificateLog(certificate.Id, action, certificate.Status, certificate.DeletedAt.Value,
                    certificate.CertificateData, certificate.DeletedBy, certificate.BatchNumber, reasonForChange);
            }

            await _context.SaveChangesAsync();
        }

        private static void UpdateIncidentNumber(string incidentNumber, Certificate cert)
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(cert.CertificateData);
            certificateData.IncidentNumber = incidentNumber;
            cert.CertificateData = JsonConvert.SerializeObject(certificateData);
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

        private async Task AddCertificateLog(Guid certificateId, string action, string status, DateTime eventTime, string certificateData, string username, int? batchNumber, string reasonForChange = null)
        {
            if (action != null)
            {
                var certLog = new CertificateLog
                {
                    Id = Guid.NewGuid(),
                    CertificateId = certificateId,
                    Action = action,
                    Status = status,
                    EventTime = eventTime,
                    CertificateData = certificateData,
                    Username = username,
                    BatchNumber = batchNumber,
                    ReasonForChange = reasonForChange
                };

                await _context.CertificateLogs.AddAsync(certLog);
            }
        }

        public async Task UpdatePrintStatus(Certificate certificate, int batchNumber, string printStatus, DateTime statusAt, string reasonForChange, bool changesCertificateStatus)
        {
            var certificateBatchLog =
                await _context.CertificateBatchLogs.FirstOrDefaultAsync(
                    q => q.CertificateReference == certificate.CertificateReference && q.BatchNumber == batchNumber);

            if (certificateBatchLog != null && certificateBatchLog.Status != printStatus)
            {
                if (changesCertificateStatus)
                {
                    certificate.BatchNumber = batchNumber;
                    certificate.Status = printStatus;
                    certificate.UpdatedBy = SystemUsers.PrintFunction;
                }

                if (statusAt >= certificateBatchLog.StatusAt)
                {
                    certificateBatchLog.Status = printStatus;
                    certificateBatchLog.StatusAt = statusAt;
                    certificateBatchLog.ReasonForChange = reasonForChange;
                    certificateBatchLog.UpdatedBy = SystemUsers.PrintFunction;
                }

                var action = (printStatus == CertificateStatus.Printed ? CertificateActions.Printed : CertificateActions.Status);
                
                await AddCertificateLog(certificate.Id, action, printStatus, statusAt,
                    certificateBatchLog.CertificateData, SystemUsers.PrintFunction, 
                    certificateBatchLog.BatchNumber, reasonForChange);
                
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateSentToPrinter(Certificate certificate, int batchNumber, DateTime sentToPrinterDate)
        {
            if (certificate != null)
            {
                var certificateBatchLog =
                    await _context.CertificateBatchLogs.FirstOrDefaultAsync(
                        q => q.CertificateReference == certificate.CertificateReference && q.BatchNumber == batchNumber);

                if (certificateBatchLog == null)
                {
                    await _context.CertificateBatchLogs.AddAsync(new CertificateBatchLog
                    {
                        CertificateReference = certificate.CertificateReference,
                        BatchNumber = batchNumber,
                        CertificateData = certificate.CertificateData,
                        Status = CertificateStatus.SentToPrinter,
                        StatusAt = sentToPrinterDate,
                        CreatedBy = SystemUsers.PrintFunction
                    });

                    certificate.BatchNumber = batchNumber;
                    certificate.Status = CertificateStatus.SentToPrinter;
                    certificate.ToBePrinted = sentToPrinterDate;
                    certificate.UpdatedBy = SystemUsers.PrintFunction;

                    await AddCertificateLog(certificate.Id, CertificateActions.Status, certificate.Status, certificate.ToBePrinted.Value,
                            certificate.CertificateData, SystemUsers.PrintFunction, certificate.BatchNumber);

                    await _context.SaveChangesAsync();
                }
            }            
        }

        public async Task<List<Certificate>> GetCertificatesForBatchLog(int batchNumber)
        {
            var certificaes = await (from certificateBatchLog in _context.CertificateBatchLogs
                                     join certificate in _context.Certificates on
                                          certificateBatchLog.CertificateReference equals certificate.CertificateReference
                                     where certificateBatchLog.BatchNumber == batchNumber
                                     select certificate).ToListAsync();

            return certificaes;
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
            var statuses = new[] { CertificateStatus.Submitted }.Concat(CertificateStatus.PrintProcessStatus).ToList();

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

        public async Task ApproveCertificates(List<ApprovalResult> approvalResults, string username)
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
                            var certData = JsonConvert.DeserializeObject<CertificateData>(certLog.CertificateData);
                            var deleted =  _context.CertificateLogs.Any(x => x.CertificateId == certificate.Id && x.Status == CertificateStatus.Deleted);
                            if (!deleted && certData?.OverallGrade != CertificateGrade.Fail)
                                continue;
                        }
                    }

                    certificate.Status = approvalResult.IsApproved;
                    certificate.PrivatelyFundedStatus = approvalResult.PrivatelyFundedStatus;

                    await AddCertificateLog(certificate.Id, certificate.PrivatelyFundedStatus, certificate.Status, DateTime.UtcNow,
                        certificate.CertificateData, username, certificate.BatchNumber, approvalResult.ReasonForChange);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}