using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.DTOs.Certificate;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Paging;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;




namespace SFA.DAS.AssessorService.Data
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly IAssessorUnitOfWork _unitOfWork;

        public CertificateRepository(IAssessorUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Certificate> NewStandardCertificate(Certificate certificate)
        {
            var query = _unitOfWork.AssessorDbContext.StandardCertificates
                .Where(c =>
                    c.Uln == certificate.Uln &&
                    c.StandardCode == certificate.StandardCode);

            var existingCertificate = await query.FirstOrDefaultAsync();
            return existingCertificate == null ? await CreateCertificate(certificate) : existingCertificate;
        }

        public async Task<FrameworkCertificate> NewFrameworkCertificate(FrameworkCertificate certificate)
        {
            var query = _unitOfWork.AssessorDbContext.FrameworkCertificates
                .Where(c =>
                    c.FrameworkLearnerId == certificate.FrameworkLearnerId);

            var existingCertificate = await query.FirstOrDefaultAsync();
            return existingCertificate == null ? await CreateCertificate(certificate) : existingCertificate;
        }

        private async Task<T> CreateCertificate<T>(T certificate) where T : CertificateBase
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                certificate.Id = Guid.NewGuid();
                await _unitOfWork.AssessorDbContext.Set<CertificateBase>().AddAsync(certificate);

                // generate the IDENTITY value from the CertificateReferenceId
                await _unitOfWork.AssessorDbContext.SaveChangesAsync();

                // format the IDENITY value into the CertificateReference - note that this is not using a sequence
                // or a computed column because doing so at this stage would require a large amount of rework in the
                // database schema
                certificate.CertificateReference = certificate.CertificateReferenceId.ToString().PadLeft(8, '0');

                // ensure the EpaDetails are kept in sychronization with the certificate reference
                certificate.CertificateData.EpaDetails.EpaReference = certificate.CertificateReference;

                AddSingleCertificateLog(certificate.Id, CertificateActions.Start, certificate.Status, DateTime.UtcNow,
                    certificate.CertificateData, certificate.CreatedBy, certificate.BatchNumber);
            });

            return certificate;
        }

        public async Task<T> GetCertificate<T>(Guid id, bool includeLogs = false) where T : CertificateBase
        {
            IQueryable<CertificateBase> query = _unitOfWork.AssessorDbContext.Set<T>();

            if (includeLogs)
            {
                query = query.Include(c => c.CertificateLogs);
            }

            return await query.OfType<T>().SingleOrDefaultAsync(c => c.Id == id);
        }


        public async Task<Certificate> GetCertificate(long uln, int standardCode)
        {
            return await _unitOfWork.AssessorDbContext.StandardCertificates
                 .Include(q => q.CertificateBatchLog)
                 .SingleOrDefaultAsync(c =>
                    c.Uln == uln && 
                    c.StandardCode == standardCode);
        }

        public async Task<Certificate> GetCertificate(long uln, int standardCode, string familyName, bool includeLogs = false)
        {
            var query = _unitOfWork.AssessorDbContext.StandardCertificates
                .Include(q => q.CertificateBatchLog)
                .AsQueryable();

            if (includeLogs)
            {
                query = query.Include(c => c.CertificateLogs);
            }

            return await query
                .SingleOrDefaultAsync(c =>
                    c.Uln == uln &&
                    c.StandardCode == standardCode &&
                    c.LearnerFamilyName.Equals(familyName));
        }

        public async Task<Certificate> GetCertificate(long uln, int standardCode, string familyName, string endpointOrganisationId)
        {
            return await _unitOfWork.AssessorDbContext.StandardCertificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c =>
                    c.Uln == uln &&
                    c.StandardCode == standardCode &&
                    c.LearnerFamilyName.Equals(familyName) &&
                    c.Organisation.EndPointAssessorOrganisationId == endpointOrganisationId);
        }

        public async Task<Certificate> GetCertificate(long uln, string familyName)
        {
            var certificate = await _unitOfWork.AssessorDbContext.StandardCertificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c =>
                    c.Uln == uln &&
                    c.LearnerFamilyName.Equals(familyName));

            return certificate;
        }

        public async Task<T> GetCertificate<T>(string certificateReference, string familyName, DateTime? achievementDate) where T : CertificateBase
        {
            IQueryable<CertificateBase> query = _unitOfWork.AssessorDbContext.Set<T>();

            return await query.OfType<T>().FirstOrDefaultAsync(c =>
                c.CertificateReference == certificateReference &&
                c.LearnerFamilyName.Equals(familyName) &&
                c.AchievementDate.Equals(achievementDate));
        }

        public async Task<T> GetCertificate<T>(string certificateReference) where T : CertificateBase
        {
            IQueryable<CertificateBase> query = _unitOfWork.AssessorDbContext.Set<T>();

            return await query.OfType<T>().FirstOrDefaultAsync(c => 
                c.CertificateReference == certificateReference);
        }

        public async Task<bool> CertificateExistsForUln(long uln)
        {
            return await _unitOfWork.AssessorDbContext.StandardCertificates
                .AnyAsync(c => c.Uln == uln);
        }

        public async Task<Certificate> GetCertificateDeletedByUln(long uln)
        {
            return await _unitOfWork.AssessorDbContext.StandardCertificates
                .Include(q => q.Organisation)
                .FirstOrDefaultAsync(c =>
                    c.Uln == uln &&
                    c.Status == CertificateStatus.Deleted);
        }


        public async Task<List<Certificate>> GetDraftAndCompletedCertificatesFor(long uln)
        {
            var statuses = new[] { CertificateStatus.Draft, CertificateStatus.Submitted, CertificateStatus.ToBeApproved }.Concat(CertificateStatus.PrintProcessStatus).ToList();
            return await _unitOfWork.AssessorDbContext.StandardCertificates.Where(c => c.Uln == uln && statuses.Contains(c.Status))
                .Include(c => c.CertificateLogs)
                .ToListAsync();
        }

        public async Task<FrameworkCertificate> GetFrameworkCertificate(Guid frameworkLearnerId)
        {
            return await _unitOfWork.AssessorDbContext.FrameworkCertificates
                 .Include(q => q.CertificateBatchLog)
                 .SingleOrDefaultAsync(c => c.FrameworkLearnerId == frameworkLearnerId);
        }

        public async Task<int> GetCertificatesReadyToPrintCount(string[] excludedOverallGrades, string[] includedStatus)
        {
            var query = _unitOfWork.AssessorDbContext
                .Set<CertificateBase>()
                .Where(c => c.CertificateData.ContactAddLine1 != null &&
                            c.CertificateData.ContactPostCode != null &&
                            !excludedOverallGrades.Contains(c.CertificateData.OverallGrade) &&
                            includedStatus.Contains(c.Status) &&
                            c.BatchNumber == null);

            return await query.CountAsync();
        }

        public async Task<Guid[]> GetCertificatesReadyToPrint(int numberOfCertificates, string[] excludedOverallGrades, string[] includedStatus)
        {
            var query = _unitOfWork.AssessorDbContext
                .Set<CertificateBase>()
                .Where(c => c.CertificateData.ContactAddLine1 != null &&
                            c.CertificateData.ContactPostCode != null &&
                            !excludedOverallGrades.Contains(c.OverallGrade) &&
                            includedStatus.Contains(c.Status) &&
                            c.BatchNumber == null)
                .OrderBy(c => c.Id)
                .Take(numberOfCertificates)
                .Select(c => c.Id);

            return await query.ToArrayAsync();
        }

        public async Task UpdateCertificatesReadyToPrintInBatch(Guid[] certificateIds, int batchNumber)
        {
            var certificates = await _unitOfWork.AssessorDbContext.Set<CertificateBase>()
                .Where(c => certificateIds.Contains(c.Id))
                .ToListAsync();

            foreach (var certificate in certificates)
            {
                certificate.UpdatedAt = DateTime.UtcNow;
                certificate.UpdatedBy = SystemUsers.PrintFunction;
                certificate.BatchNumber = batchNumber;
            }

            await AddMultipleCertificateLogs(certificateIds, CertificateActions.Status, SystemUsers.PrintFunction, batchNumber, null);
            await _unitOfWork.AssessorDbContext.SaveChangesAsync();
        }

        public async Task<PaginatedList<CertificateHistoryModel>> GetCertificateHistory(string endPointAssessorOrganisationId,
                int pageIndex,
                int pageSize,
                string searchTerm,
                string sortColumn,
                bool sortDescending,
                List<string> statuses)
        {
            var certificatesQuery = _unitOfWork.AssessorDbContext.StandardCertificates
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
                     ProviderUkPrn = q.ProviderUkPrn.GetValueOrDefault(),
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

        private static IQueryable<Certificate> GetSortedCertificateInfo(bool sortDescending, GetCertificateHistoryRequest.SortColumns sort, IQueryable<Certificate> certificatesQuery)
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

        public async Task<Certificate> UpdateStandardCertificate(Certificate updatedCertificate, string username, string action, bool updateLog = true, string reasonForChange = null)
        {
            var certificate = await GetCertificate<Certificate>(updatedCertificate.Id)
                ?? throw new NotFoundException();

            certificate.Uln = updatedCertificate.Uln;
            certificate.StandardUId = updatedCertificate.StandardUId;
            certificate.CertificateData = CloneCertificateData(updatedCertificate.CertificateData);
            certificate.Status = updatedCertificate.Status;
            certificate.ProviderUkPrn = updatedCertificate.ProviderUkPrn;
            certificate.StandardCode = updatedCertificate.StandardCode;
            certificate.UpdatedBy = username;
            certificate.UpdatedAt = DateTime.UtcNow;
            certificate.IsPrivatelyFunded = updatedCertificate.IsPrivatelyFunded;
            certificate.StandardUId = updatedCertificate.StandardUId;

            if (certificate.IsPrivatelyFunded)
            {
                certificate.PrivatelyFundedStatus = updatedCertificate.PrivatelyFundedStatus;
            }

            if (certificate.Status != CertificateStatus.Deleted)
            {
                certificate.DeletedBy = null;
                certificate.DeletedAt = null;
            }

            certificate.ToBePrinted = null;
            certificate.BatchNumber = null;

            if (updateLog)
            {
                AddSingleCertificateLog(certificate.Id, action, certificate.Status, certificate.UpdatedAt.Value,
                    certificate.CertificateData, certificate.UpdatedBy, certificate.BatchNumber, reasonForChange);
            }

            await _unitOfWork.AssessorDbContext.SaveChangesAsync();

            return certificate;
        }

        public async Task<FrameworkCertificate> UpdateFrameworkCertificate(FrameworkCertificate updatedCertificate, string username, string action)
        {
            var certificate = await GetCertificate<FrameworkCertificate>(updatedCertificate.Id)
                ?? throw new NotFoundException();

            certificate.CertificateData = CloneCertificateData(updatedCertificate.CertificateData);
            certificate.Status = updatedCertificate.Status;
            certificate.UpdatedBy = username;
            certificate.UpdatedAt = DateTime.UtcNow;

            certificate.ToBePrinted = null;
            certificate.BatchNumber = null;

            AddSingleCertificateLog(certificate.Id, action, certificate.Status, certificate.UpdatedAt.Value,
                certificate.CertificateData, certificate.UpdatedBy, certificate.BatchNumber, null);

            await _unitOfWork.AssessorDbContext.SaveChangesAsync();

            return certificate;
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
                certificate.CertificateData.IncidentNumber = incidentNumber;
            }

            if (updateLog)
            {
                AddSingleCertificateLog(certificate.Id, action, certificate.Status, certificate.DeletedAt.Value,
                    certificate.CertificateData, certificate.DeletedBy, certificate.BatchNumber, reasonForChange);
            }

            await _unitOfWork.AssessorDbContext.SaveChangesAsync();
        }

        public async Task<CertificateBase> UpdateProviderName(Guid certificateId, string providerName)
        {
            var certificate = await GetCertificate<Certificate>(certificateId)
                ?? throw new NotFoundException();

            certificate.CertificateData.ProviderName = providerName;

            await _unitOfWork.AssessorDbContext.SaveChangesAsync();

            return certificate;
        }

        public async Task UpdatePrintStatus(Guid certificateId, int batchNumber, string printStatus, DateTime statusAt, string reasonForChange,
            bool updateCertificate, bool updateCertificateBatchLog)
        {
            var certificate = await GetCertificate<CertificateBase>(certificateId)
                ?? throw new NotFoundException();

            var certificateBatchLog =
                await _unitOfWork.AssessorDbContext.CertificateBatchLogs.FirstOrDefaultAsync(
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

            await _unitOfWork.AssessorDbContext.SaveChangesAsync();
        }

        public async Task<List<CertificateLog>> GetCertificateLogsFor(Guid certificateId)
        {
            return await _unitOfWork.AssessorDbContext.CertificateLogs
                .Where(l => l.CertificateId == certificateId)
                .OrderByDescending(l => l.EventTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CertificateAddress> GetContactPreviousAddress(string epaOrgId, long? employerAccountId)
        {
            if (string.IsNullOrEmpty(epaOrgId) || employerAccountId == null)
                return null;
            
            var statuses = new[] { CertificateStatus.Submitted }
                .Concat(CertificateStatus.PrintProcessStatus)
                .ToList();

            var query = _unitOfWork.AssessorDbContext.StandardCertificates
                .Include(c => c.Organisation)
                .Where(c =>
                    c.Organisation.EndPointAssessorOrganisationId == epaOrgId &&
                    c.CertificateData.EmployerAccountId == employerAccountId &&
                    c.CertificateData.SendTo == CertificateSendTo.Employer &&
                    statuses.Contains(c.Status))
                .OrderByDescending(c => c.UpdatedBy != null ? c.UpdatedAt : c.CreatedAt)

                .Select(c => new CertificateAddress
                {
                    OrganisationId = c.OrganisationId,
                    ContactOrganisation = c.CertificateData.ContactOrganisation,
                    ContactName = c.CertificateData.ContactName,
                    Department = c.CertificateData.Department,
                    AddressLine1 = c.CertificateData.ContactAddLine1,
                    AddressLine2 = c.CertificateData.ContactAddLine2,
                    AddressLine3 = c.CertificateData.ContactAddLine3,
                    City = c.CertificateData.ContactAddLine4,
                    PostCode = c.CertificateData.ContactPostCode
                });

            return await query.FirstOrDefaultAsync();
        }

        public async Task<string> GetPreviousProviderName(int providerUkPrn)
        {
            var query = _unitOfWork.AssessorDbContext.StandardCertificates
                .Where(c => c.ProviderUkPrn == providerUkPrn &&
                            c.CertificateData.ProviderName != null)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => c.CertificateData.ProviderName);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<AssessmentsResult> GetAssessments(long ukprn, string standardReference)
        {
            var query = _unitOfWork.AssessorDbContext.StandardCertificates
                .Where(c => !c.IsPrivatelyFunded &&
                            !(c.Status == CertificateStatus.Deleted || c.Status == CertificateStatus.Draft) &&
                            c.ProviderUkPrn == ukprn &&
                            (standardReference == null || c.StandardReference == standardReference))
                .GroupBy(c => 1)
                .Select(g => new AssessmentsResult
                {
                    EarliestAssessment = g.Min(c => c.AchievementDate ?? DateTime.MaxValue),
                    EndpointAssessmentCount = g.Count()
                });

            return await query.FirstOrDefaultAsync() ?? new AssessmentsResult
            {
                EarliestAssessment = null,
                EndpointAssessmentCount = 0
            };
        }

        private void AddSingleCertificateLog(Guid certificateId, string action, string status, DateTime eventTime, CertificateData certificateData, string username, int? batchNumber, string reasonForChange = null)
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
                    CertificateData = CloneCertificateData(certificateData),
                    Username = username,
                    BatchNumber = batchNumber,
                    ReasonForChange = reasonForChange
                };

                _unitOfWork.AssessorDbContext.CertificateLogs.Add(certLog);
            }
        }

        private async Task AddMultipleCertificateLogs(Guid[] certificateIds, string action, string username, int? batchNumber, string reasonForChange = null)
        {
            var certificates = await _unitOfWork.AssessorDbContext.Set<CertificateBase>()
                .Where(c => certificateIds.Contains(c.Id))
                .ToListAsync();

            var logs = certificates.Select(c => new CertificateLog
            {
                Id = Guid.NewGuid(),
                Action = action,
                CertificateId = c.Id,
                EventTime = c.UpdatedAt.Value,
                Status = c.Status,
                CertificateData = CloneCertificateData(c.CertificateData),
                Username = username,
                BatchNumber = batchNumber,
                ReasonForChange = reasonForChange
            }).ToList();

            await _unitOfWork.AssessorDbContext.CertificateLogs.AddRangeAsync(logs);
        }

		public async Task UpdateAssessmentsSummary()
		{
			await _unitOfWork.AssessorDbContext.ExecuteStoredProcedureAsync(
				"AssessmentsSummaryUpdate",
				param: null,
				commandTimeout: 0,
				commandType: CommandType.StoredProcedure);
		}

		private static CertificateData CloneCertificateData(CertificateData original)
        {
            var serialized = JsonConvert.SerializeObject(original);
            return JsonConvert.DeserializeObject<CertificateData>(serialized);
        }

    }
}