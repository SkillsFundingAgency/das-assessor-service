using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Data
{
    public class CertificateRepository : Repository, ICertificateRepository
    {
        private readonly AssessorDbContext _context;
        
        public CertificateRepository(IUnitOfWork unitOfWork, AssessorDbContext context)
            : base(unitOfWork)
        {
            _context = context;
            
            SqlMapper.AddTypeHandler(typeof(CertificateData), new CertificateDataHandler());
        }

        public async Task<Certificate> New(Certificate certificate)
        {
            // cannot create a New certificate for same uln / std code on the same day - return existing
            var existingCert = await _context.Certificates
                .FirstOrDefaultAsync(c =>
                    c.Uln == certificate.Uln && 
                    c.StandardCode == certificate.StandardCode && 
                    c.CreateDay == certificate.CreateDay);
            
            if (existingCert != null) return existingCert;
            
            try
            {
                return await CreateCertificate(certificate);
            }
            catch (Exception e)
            {
                if (!(e.InnerException is SqlException sqlException)) throw;

                if (sqlException.Number == 2601 || sqlException.Number == 2627)
                {
                    // cannot create a New certificate for same uln / std code on the same day - return existing
                    return await _context.Certificates.FirstOrDefaultAsync(c =>
                        c.Uln == certificate.Uln && 
                        c.StandardCode == certificate.StandardCode && 
                        c.CreateDay == certificate.CreateDay);
                }

                throw;
            }
        }

        private async Task<Certificate> CreateCertificate(Certificate certificate)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        certificate.Id = Guid.NewGuid();
                        await _context.Certificates.AddAsync(certificate);

                        // generate the IDENTITY value from the CertificateReferenceId
                        await _context.SaveChangesAsync();

                        // format the IDENITY value into the CertificateReference - note that this is not using a sequence
                        // or a computed column because doing so at this stage would require a large amount of rework in the
                        // database schema
                        certificate.CertificateReference = certificate.CertificateReferenceId.ToString().PadLeft(8, '0');

                        // ensure the EpaDetails are kept in sychronization with the certificate reference
                        var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                        certificateData.EpaDetails.EpaReference = certificate.CertificateReference;
                        certificate.CertificateData = JsonConvert.SerializeObject(certificateData);

                        await AddSingleCertificateLog(certificate.Id, CertificateActions.Start, certificate.Status, DateTime.UtcNow,
                            certificate.CertificateData, certificate.CreatedBy, certificate.BatchNumber);

                        await _context.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            });

            return certificate;
        }

        public async Task<Certificate> GetCertificate(Guid id, bool includeLogs = false)
        {
            if (!includeLogs)
            {
                return await _context.Certificates
                    .SingleOrDefaultAsync(c => c.Id == id);
            }

            return await _context.Certificates
                .Include(l => l.CertificateLogs)
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Certificate> GetCertificate(long uln, int standardCode)
        {
            return await _context.Certificates
                .Include(q => q.CertificateBatchLog)
                .SingleOrDefaultAsync(c =>
                c.Uln == uln && c.StandardCode == standardCode);
        }

        public async Task<Certificate> GetCertificate(long uln, int standardCode, string familyName)
        {
            var certificate = await GetCertificate(uln, standardCode);
            
            if (certificate is null)
                return certificate;

            return CheckCertificateData(certificate, familyName) ? certificate : null;
        }

        public async Task<Certificate> GetCertificateByUlnOrgIdLastnameAndStandardCode(long uln,
            string endpointOrganisationId, string lastName, int standardCode)
        {
            var existingCert = await _context.Certificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c =>
                    c.Uln == uln &&
                    c.Organisation.EndPointAssessorOrganisationId == endpointOrganisationId &&
                    c.StandardCode == standardCode &&
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
            
            return certificateData.LearnerFamilyName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase);            
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

        public async Task<List<Certificate>> GetDraftAndCompletedCertificatesFor(long uln)
        {
            var statuses = new[] { CertificateStatus.Draft, CertificateStatus.Submitted, CertificateStatus.ToBeApproved }.Concat(CertificateStatus.PrintProcessStatus).ToList();
            return await _context.Certificates.Where(c => c.Uln == uln && statuses.Contains(c.Status))
                .ToListAsync();
        }

        public async Task<int> GetCertificatesReadyToPrintCount(string[] excludedOverallGrades, string[] includedStatus)
        {
            var sql = 
              @"SELECT COUNT(1) 
                FROM 
                    [Certificates] c 
                WHERE 
                    JSON_VALUE(CertificateData, '$.ContactAddLine1') IS NOT NULL 
                    AND JSON_VALUE(CertificateData, '$.ContactPostCode') IS NOT NULL 
                    AND JSON_VALUE(CertificateData, '$.OverallGrade') NOT IN @excludedOverallGrades 
                    AND c.Status IN @includedStatus 
                    AND c.BatchNumber IS NULL";

            var count = await _unitOfWork.Connection.QueryFirstAsync<int>(
                sql,
                param: new { excludedOverallGrades, includedStatus },
                transaction: _unitOfWork.Transaction);

            return count;
        }

        public async Task<Guid[]> GetCertificatesReadyToPrint(int numberOfCertificates, string[] excludedOverallGrades, string[] includedStatus )
        {
            var certificateIds = await _unitOfWork.Connection.QueryAsync<Guid>(
              @"SELECT TOP(@numberOfCertificates) 
                    c.[Id] 
                FROM 
                    [Certificates] c 
                WHERE 
                    JSON_VALUE(CertificateData, '$.ContactAddLine1') IS NOT NULL 
                    AND JSON_VALUE(CertificateData, '$.ContactPostCode') IS NOT NULL 
                    AND JSON_VALUE(CertificateData, '$.OverallGrade') NOT IN @excludedOverallGrades 
                    AND c.Status IN @includedStatus 
                    AND BatchNumber IS NULL",
                param: new { numberOfCertificates, excludedOverallGrades, includedStatus },
                transaction: _unitOfWork.Transaction);

            return certificateIds.ToArray();
        }

        public async Task UpdateCertificatesReadyToPrintInBatch(Guid[] certificateIds, int batchNumber)
        {
            var sql =
              @"UPDATE [Certificates] SET 
                    UpdatedAt = GETUTCDATE(), 
                    UpdatedBy = @updatedBy, 
                    BatchNumber = @batchNumber 
                WHERE 
                    Id IN @certificateIds";

            await _unitOfWork.Connection.ExecuteAsync(
                sql,
                param: new { certificateIds, batchNumber, updatedBy = SystemUsers.PrintFunction },
                transaction: _unitOfWork.Transaction);

            await AddMultipleCertificateLogs(certificateIds, CertificateActions.Status, null, null, null, SystemUsers.PrintFunction, batchNumber, null);
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
            var cert = await GetCertificate(certificate.Id)
                ?? throw new NotFound();

            cert.Uln = certificate.Uln;
            cert.StandardUId = certificate.StandardUId;
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

            cert.ToBePrinted = null;
            cert.BatchNumber = null;

            if (updateLog)
            {
                await AddSingleCertificateLog(cert.Id, action, cert.Status, cert.UpdatedAt.Value,
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
                await AddSingleCertificateLog(certificate.Id, action, certificate.Status, certificate.DeletedAt.Value,
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

        public async Task UpdatePrintStatus(Certificate certificate, int batchNumber, string printStatus, DateTime statusAt, string reasonForChange, bool isLatestChange)
        {
            var certificateBatchLog =
                await _context.CertificateBatchLogs.FirstOrDefaultAsync(
                    q => q.CertificateReference == certificate.CertificateReference && q.BatchNumber == batchNumber);

            if (certificateBatchLog != null && certificateBatchLog.Status != printStatus)
            {
                if (isLatestChange)
                {
                    certificate.BatchNumber = batchNumber;
                    certificate.Status = printStatus;
                    certificate.UpdatedBy = SystemUsers.PrintFunction;

                    if(printStatus == CertificateStatus.SentToPrinter)
                    {
                        certificate.ToBePrinted = statusAt;
                    }
                }

                if (statusAt >= certificateBatchLog.StatusAt)
                {
                    certificateBatchLog.Status = printStatus;
                    certificateBatchLog.StatusAt = statusAt;
                    certificateBatchLog.ReasonForChange = reasonForChange;
                    certificateBatchLog.UpdatedBy = SystemUsers.PrintFunction;
                }

                var action = (printStatus == CertificateStatus.Printed ? CertificateActions.Printed : CertificateActions.Status);
                
                await AddSingleCertificateLog(certificate.Id, action, printStatus, statusAt,
                    certificateBatchLog.CertificateData, SystemUsers.PrintFunction, 
                    certificateBatchLog.BatchNumber, reasonForChange);
                
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CertificateLog>> GetCertificateLogsFor(Guid certificateId)
        {
            return await _context.CertificateLogs.Where(l => l.CertificateId == certificateId).OrderByDescending(l => l.EventTime)
                .AsNoTracking()
                .ToListAsync();
        }
        
        public async Task<CertificateAddress> GetContactPreviousAddress(string username)
        {
            var statuses = new[] { CertificateStatus.Submitted }.Concat(CertificateStatus.PrintProcessStatus).ToList();

            var certificateAddress = await (from certificate in _context.Certificates
                                     where 
                                        statuses.Contains(certificate.Status) 
                                        && certificate.UpdatedBy == username
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
            var sql =
              @"SELECT 
                   TOP(1) JSON_VALUE(CertificateData, '$.ProviderName') 
                FROM 
                   Certificates 
                WHERE 
                   ProviderUkPrn = @providerUkPrn 
                   AND JSON_VALUE(CertificateData, '$.ProviderName') IS NOT NULL 
                ORDER BY 
                   CreatedAt DESC";

            return _unitOfWork.Connection.QueryFirstOrDefaultAsync<string>(
                sql,
                param: new {providerUkPrn},
                transaction: _unitOfWork.Transaction);
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

                    await AddSingleCertificateLog(certificate.Id, certificate.PrivatelyFundedStatus, certificate.Status, DateTime.UtcNow,
                        certificate.CertificateData, username, certificate.BatchNumber, approvalResult.ReasonForChange);
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task AddSingleCertificateLog(Guid certificateId, string action, string status, DateTime eventTime, string certificateData, string username, int? batchNumber, string reasonForChange = null)
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

        private async Task AddMultipleCertificateLogs(Guid[] certificateIds, string action, string status, DateTime? eventTime, string certificateData, string username, int? batchNumber, string reasonForChange = null)
        {
            var sql =
                $@"INSERT INTO [CertificateLogs] 
                   (
                       Id, 
                       Action, 
                       CertificateId, 
                       EventTime, 
                       Status, 
                       CertificateData, 
                       Username, 
                       BatchNumber, 
                       ReasonForChange
                   ) 
                   SELECT 
                       NEWID(), 
                       @action, 
                       c.Id, 
                       {(eventTime.HasValue ? "@eventTime" : "c.UpdatedAt")}, 
                       {(!string.IsNullOrEmpty(status) ? "@status" : "c.Status")}, 
                       {(!string.IsNullOrEmpty(certificateData) ? "@certificateData" : "c.CertificateData")}, 
                       @username, 
                       @batchNumber, 
                       @reasonForChange 
                  FROM 
                       [Certificates] c 
                   WHERE 
                       c.Id IN @certificateIds";

            await _unitOfWork.Connection.ExecuteAsync(
                   sql,
                   param: new { certificateIds, action, status, eventTime, certificateData, username, batchNumber, reasonForChange },
                   transaction: _unitOfWork.Transaction);
        }
    }
}