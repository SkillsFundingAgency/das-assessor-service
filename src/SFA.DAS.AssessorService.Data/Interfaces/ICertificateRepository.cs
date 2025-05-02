using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.DTOs.Certificate;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface ICertificateRepository
    {
        Task<Certificate> NewStandardCertificate(Certificate certificate);
        Task<FrameworkCertificate> NewFrameworkCertificate(FrameworkCertificate certificate);

        Task<T> GetCertificate<T>(Guid id, bool includeLogs = false) where T : CertificateBase;

        Task<Certificate> GetCertificate(long uln, int standardCode);

        Task<Certificate> GetCertificate(long uln, int standardCode, string familyName, bool includeLogs = false);

        Task<Certificate> GetCertificate(long uln, int standardCode, string familyName, string endpointOrganisationId);

        Task<Certificate> GetCertificate(long uln, string familyName);

        Task<T> GetCertificate<T>(string certificateReference) where T : CertificateBase;

        Task<T> GetCertificate<T>(string certificateReference, string familyName, DateTime? achievementDate) where T : CertificateBase;

        Task<List<Certificate>> GetDraftAndCompletedCertificatesFor(long uln);

        Task<FrameworkCertificate> GetFrameworkCertificate(Guid frameworkLearnerId);

        Task<int> GetCertificatesReadyToPrintCount(string[] excludedOverallGrades, string[] includedStatus);

        Task<Guid[]> GetCertificatesReadyToPrint(int numberOfCertificates, string[] excludedOverallGrades, string[] includedStatus);

        Task UpdateCertificatesReadyToPrintInBatch(Guid[] certificateIds, int batchNumber);

        Task<Certificate> UpdateStandardCertificate(Certificate updatedCertificate, string username, string action, bool updateLog = true, string reasonForChange = null);

        Task<FrameworkCertificate> UpdateFrameworkCertificate(FrameworkCertificate updatedCertificate, string username, string action);

        Task Delete(long uln, int standardCode, string username, string action, bool updateLog = true, string reasonForChange = null, string incidentNumber = null);

        Task<CertificateBase> UpdateProviderName(Guid certificateId, string providerName);

        Task UpdatePrintStatus(Guid certificateId, int batchNumber, string printStatus, DateTime statusAt, string reasonForChange,
            bool updateCertificate, bool updateCertificateBatchLog);

        Task<List<CertificateLog>> GetCertificateLogsFor(Guid certificateId);

        Task<PaginatedList<CertificateHistoryModel>> GetCertificateHistory(string endPointAssessorOrganisationId, int pageIndex, int pageSize, string searchTerm, string sortColumn, bool sortDescending, List<string> statuses);

        Task<string> GetPreviousProviderName(int providerUkPrn);
        Task<CertificateAddress> GetContactPreviousAddress(string epaOrgId, long? employerAccountId);
        Task<bool> CertificateExistsForUln(long uln);

        Task<Certificate> GetCertificateDeletedByUln(long uln);

        Task<AssessmentsResult> GetAssessments(long ukprn, string standardReference);

        Task UpdateAssessmentsSummary();
    }
}