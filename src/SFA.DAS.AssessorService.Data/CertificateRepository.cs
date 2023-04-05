using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;
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

                        AddSingleCertificateLog(certificate.Id, CertificateActions.Start, certificate.Status, DateTime.UtcNow,
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

        public async Task<Certificate> GetCertificate(Guid id, 
            bool includeLogs = false)
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

        public async Task<Certificate> GetCertificate(long uln, 
            int standardCode)
        {
            return await _context.Certificates
                 .Include(q => q.CertificateBatchLog)
                 .SingleOrDefaultAsync(c =>
                    c.Uln == uln && 
                    c.StandardCode == standardCode);
        }

        public async Task<Certificate> GetCertificate(long uln, 
            int standardCode, string familyName, bool includeLogs = false)
        {
            if (!includeLogs)
            {
                return await _context.Certificates
                 .Include(q => q.CertificateBatchLog)
                 .SingleOrDefaultAsync(c =>
                    c.Uln == uln &&
                    c.StandardCode == standardCode &&
                    c.LearnerFamilyName.Equals(familyName));
            }
            else
            {
                return await _context.Certificates
                   .Include(q => q.CertificateBatchLog)
                   .Include(q => q.CertificateLogs)
                   .SingleOrDefaultAsync(c =>
                        c.Uln == uln &&
                        c.StandardCode == standardCode &&
                        c.LearnerFamilyName.Equals(familyName));
                
            }
        }

        public async Task<Certificate> GetCertificate(long uln,
            int standardCode, string familyName, string endpointOrganisationId)
        {
            return await _context.Certificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c =>
                    c.Uln == uln &&
                    c.StandardCode == standardCode &&
                    c.LearnerFamilyName.Equals(familyName) &&
                    c.Organisation.EndPointAssessorOrganisationId == endpointOrganisationId);
        }

        public async Task<Certificate> GetCertificate(long uln,
            string familyName)
        {
            var certificate = await _context.Certificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c =>
                    c.Uln == uln &&
                    c.LearnerFamilyName.Equals(familyName));

            return certificate;
        }

        public async Task<bool> CertifciateExistsForUln(long uln)
        {
            return await _context.Certificates
                .AnyAsync(c =>
                    c.Uln == uln);
        }

        public async Task<Certificate> GetCertificateDeletedByUln(long uln)
        {
            return await _context.Certificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c =>
                    c.Uln == uln &&
                    c.Status == CertificateStatus.Deleted);
        }

        public async Task<Certificate> GetCertificate(string certificateReference,
            string familyName,  DateTime? achievementDate)
        {
            return await _context.Certificates
                .FirstOrDefaultAsync(c =>
                    c.CertificateReference == certificateReference &&
                    c.LearnerFamilyName.Equals(familyName) &&
                    c.AchievementDate.Equals(achievementDate));
        }

        public async Task<Certificate> GetCertificate(string certificateReference)
        {
            return await _context.Certificates
                .FirstOrDefaultAsync(c => 
                    c.CertificateReference == certificateReference);
        }

        public async Task<List<Certificate>> GetDraftAndCompletedCertificatesFor(long uln)
        {
            var statuses = new[] { CertificateStatus.Draft, CertificateStatus.Submitted, CertificateStatus.ToBeApproved }.Concat(CertificateStatus.PrintProcessStatus).ToList();
            return await _context.Certificates.Where(c => c.Uln == uln && statuses.Contains(c.Status))
                .Include(c => c.CertificateLogs)
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

        public async Task<Guid[]> GetCertificatesReadyToPrint(int numberOfCertificates, string[] excludedOverallGrades, string[] includedStatus)
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

            await AddMultipleCertificateLogs(certificateIds, CertificateActions.Status, SystemUsers.PrintFunction, batchNumber, null);
        }

        public async Task<PaginatedList<CertificateHistoryModel>> GetCertificateHistory(string endPointAssessorOrganisationId,
                int pageIndex,
                int pageSize,
                string searchTerm,
                string sortColumn,
                bool sortDescending,
                List<string> statuses)
        {
            var certificatesQuery = _context.Certificates
                                    .Include(q => q.CertificateLogs)
                                    .Include(q => q.CertificateBatchLog)
                                    .Include(q => q.Organisation)
                                    .Where(o => o.Organisation.EndPointAssessorOrganisationId == endPointAssessorOrganisationId && !statuses.Contains(o.Status));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                long.TryParse(searchTerm, out long validUln);

                certificatesQuery = certificatesQuery
                    .Where(x => x.FullName.ToLower().Contains(searchTerm) ||
                                x.FullName.ToLower() == searchTerm ||
                                x.CertificateReference.ToLower() == searchTerm ||
                                x.Uln == validUln);
            }

            if (!Enum.TryParse(sortColumn, out GetCertificateHistoryRequest.SortColumns sort))
                sort = GetCertificateHistoryRequest.SortColumns.DateRequested;

            certificatesQuery = GetSortedCertificateInfo(sortDescending, sort, certificatesQuery);

            var certificates = await certificatesQuery
                 .Select(q => new CertificateHistoryModel
                 {
                     Id = q.Id,
                     AchievementDate = q.AchievementDate,
                     CertificateReference = q.CertificateReference,
                     ContactAddLine1 = q.ContactAddLine1,
                     ContactAddLine2 = q.ContactAddLine2,
                     ContactAddLine3 = q.ContactAddLine3,
                     ContactAddLine4 = q.ContactAddLine4,
                     ContactName = q.ContactName,
                     ContactOrganisation = q.ContactOrganisation,
                     ContactPostCode = q.ContactPostCode,
                     CourseOption = q.CourseOption,
                     CreateDay = q.CreateDay,
                     CreatedAt = q.CreatedAt,
                     CreatedBy = q.CreatedBy,
                     FullName = q.FullName,
                     LearnRefNumber = q.LearnRefNumber,
                     LearningStartDate = q.LearningStartDate,
                     OrganisationId = q.OrganisationId,
                     OverallGrade = q.OverallGrade,
                     ProviderName = q.ProviderName,
                     ProviderUkPrn = q.ProviderUkPrn,
                     StandardCode = q.StandardCode,
                     StandardLevel = q.StandardLevel,
                     StandardName = q.StandardName,
                     StandardReference = q.StandardReference,
                     Status = q.Status,
                     Uln = q.Uln,
                     UpdatedAt = q.UpdatedAt,
                     UpdatedBy = q.UpdatedBy,
                     Version = q.Version,
                     EndPointAssessorUkprn = q.Organisation.EndPointAssessorUkprn,
                     StatusAt = q.CertificateBatchLog.StatusAt,
                     ReasonForChange = q.CertificateBatchLog.ReasonForChange,
                     CertificateLogs = q.CertificateLogs
                 })
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            var count = await certificatesQuery.Select(x => x.Id).CountAsync();

            return new PaginatedList<CertificateHistoryModel>(certificates, count, pageIndex, pageSize);
        }

        private IQueryable<Certificate> GetSortedCertificateInfo(bool sortDescending, GetCertificateHistoryRequest.SortColumns sort, IQueryable<Certificate> certificatesQuery)
        {
            switch (sort)
            {
                case GetCertificateHistoryRequest.SortColumns.Apprentice:

                    if (sortDescending)
                    {
                        certificatesQuery = certificatesQuery
                            .OrderByDescending(x => x.FullName);
                    }
                    else
                    {
                        certificatesQuery = certificatesQuery
                            .OrderBy(x => x.FullName);
                    }

                    break;

                case GetCertificateHistoryRequest.SortColumns.Employer:
                    if (sortDescending)
                    {
                        certificatesQuery = certificatesQuery
                            .OrderByDescending(x => x.ContactOrganisation);
                    }
                    else
                    {
                        certificatesQuery = certificatesQuery
                            .OrderBy(x => x.ContactOrganisation);
                    }

                    break;

                case GetCertificateHistoryRequest.SortColumns.ProviderName:
                    if (sortDescending)
                    {
                        certificatesQuery = certificatesQuery
                            .OrderByDescending(x => x.ProviderName);
                    }
                    else
                    {
                        certificatesQuery = certificatesQuery
                            .OrderBy(x => x.ProviderName);
                    }

                    break;

                case GetCertificateHistoryRequest.SortColumns.DateRequested:
                    if (sortDescending)
                    {
                        certificatesQuery = certificatesQuery
                            .OrderByDescending(x => x.CreatedAt);
                    }
                    else
                    {
                        certificatesQuery = certificatesQuery
                            .OrderBy(x => x.CreatedAt);
                    }

                    break;
            }

            return certificatesQuery;
        }

        public async Task<Certificate> Update(Certificate certificate, string username, string action, bool updateLog = true, string reasonForChange = null)
        {
            var cert = await GetCertificate(certificate.Id)
                ?? throw new NotFoundException();

            cert.Uln = certificate.Uln;
            cert.StandardUId = certificate.StandardUId;
            cert.CertificateData = certificate.CertificateData;
            cert.Status = certificate.Status;
            cert.ProviderUkPrn = certificate.ProviderUkPrn;
            cert.StandardCode = certificate.StandardCode;
            cert.UpdatedBy = username;
            cert.UpdatedAt = DateTime.UtcNow;
            cert.IsPrivatelyFunded = certificate.IsPrivatelyFunded;
            cert.StandardUId = certificate.StandardUId;

            if (cert.IsPrivatelyFunded)
            {
                cert.PrivatelyFundedStatus = certificate.PrivatelyFundedStatus;
            }

            if (certificate.Status != CertificateStatus.Deleted)
            {
                cert.DeletedBy = null;
                cert.DeletedAt = null;
            }

            cert.ToBePrinted = null;
            cert.BatchNumber = null;

            if (updateLog)
            {
                AddSingleCertificateLog(cert.Id, action, cert.Status, cert.UpdatedAt.Value,
                    cert.CertificateData, cert.UpdatedBy, cert.BatchNumber, reasonForChange);
            }

            await _context.SaveChangesAsync();

            return cert;
        }

        public async Task Delete(long uln, int standardCode, string username, string action, bool updateLog = true, string reasonForChange = null, string incidentNumber = null)
        {
            var certificate = await GetCertificate(uln, standardCode);

            if (certificate == null) throw new NotFoundException();

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
                AddSingleCertificateLog(certificate.Id, action, certificate.Status, certificate.DeletedAt.Value,
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

        public async Task UpdatePrintStatus(Certificate certificate, int batchNumber, string printStatus, DateTime statusAt, string reasonForChange,
            bool updateCertificate, bool updateCertificateBatchLog)
        {
            var certificateBatchLog =
                await _context.CertificateBatchLogs.FirstOrDefaultAsync(
                    q => q.CertificateReference == certificate.CertificateReference && q.BatchNumber == batchNumber);

            if (certificateBatchLog == null)
                throw new ArgumentException($"Certificate {certificate.CertificateReference} not found in batch {batchNumber}.");

            if (updateCertificate)
            {
                certificate.Status = printStatus;
                certificate.UpdatedBy = SystemUsers.PrintFunction;

                if (printStatus == CertificateStatus.SentToPrinter)
                {
                    certificate.ToBePrinted = statusAt;
                }
            }

            if (updateCertificateBatchLog)
            {
                certificateBatchLog.Status = printStatus;
                certificateBatchLog.StatusAt = statusAt;
                certificateBatchLog.ReasonForChange = reasonForChange;
                certificateBatchLog.UpdatedBy = SystemUsers.PrintFunction;
            }

            var action = (printStatus == CertificateStatus.Printed ? CertificateActions.Printed : CertificateActions.Status);

            AddSingleCertificateLog(certificate.Id, action, printStatus, statusAt,
                certificateBatchLog.CertificateData, SystemUsers.PrintFunction,
                certificateBatchLog.BatchNumber, reasonForChange);

            await _context.SaveChangesAsync();
        }

        public async Task<List<CertificateLog>> GetCertificateLogsFor(Guid certificateId)
        {
            return await _context.CertificateLogs.Where(l => l.CertificateId == certificateId).OrderByDescending(l => l.EventTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CertificateAddress> GetContactPreviousAddress(string epaOrgId, string employerAccountId)
        {
            var statuses = new[] { CertificateStatus.Submitted }.Concat(CertificateStatus.PrintProcessStatus).ToList();
            var sendToEmployer = nameof(CertificateSendTo.Employer);

            var sql = @"
                SELECT
                    c.OrganisationId,
                    JSON_VALUE(CertificateData, '$.ContactOrganisation') ContactOrganisation,
                    JSON_VALUE(CertificateData, '$.ContactName') ContactName,
                    JSON_VALUE(CertificateData, '$.Department') Department,
                    JSON_VALUE(CertificateData, '$.ContactAddLine1') AddressLine1 ,
                    JSON_VALUE(CertificateData, '$.ContactAddLine2') AddressLine2,
                    JSON_VALUE(CertificateData, '$.ContactAddLine3') AddressLine3,
                    JSON_VALUE(CertificateData, '$.ContactAddLine4') City,
                    JSON_VALUE(CertificateData, '$.ContactPostCode') PostCode
                FROM
                    Certificates c INNER JOIN Organisations o
                    ON c.OrganisationId = o.Id
                WHERE
                    o.EndPointAssessorOrganisationId = @epaOrgId
                    AND JSON_VALUE(CertificateData, '$.EmployerAccountId') = @employerAccountId
                    AND JSON_VALUE(CertificateData, '$.SendTo') = @sendToEmployer
                    AND c.Status IN @statuses
                ORDER BY
                   ISNULL(c.UpdatedBy, c.CreatedAt) DESC";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<CertificateAddress>(
                sql,
                param: new { epaOrgId, employerAccountId, sendToEmployer, statuses },
                transaction: _unitOfWork.Transaction);
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
                param: new { providerUkPrn },
                transaction: _unitOfWork.Transaction);
        }

        private void AddSingleCertificateLog(Guid certificateId, string action, string status, DateTime eventTime, string certificateData, string username, int? batchNumber, string reasonForChange = null)
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

                _context.CertificateLogs.Add(certLog);
            }
        }

        private async Task AddMultipleCertificateLogs(Guid[] certificateIds, string action, string username, int? batchNumber, string reasonForChange = null)
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
                       c.UpdatedAt,
                       c.Status,
                       c.CertificateData,
                       @username,
                       @batchNumber,
                       @reasonForChange
                  FROM
                       [Certificates] c
                  WHERE
                       c.Id IN @certificateIds";

            await _unitOfWork.Connection.ExecuteAsync(
                   sql,
                   param: new { certificateIds, action, username, batchNumber, reasonForChange },
                   transaction: _unitOfWork.Transaction);
        }
    }
}