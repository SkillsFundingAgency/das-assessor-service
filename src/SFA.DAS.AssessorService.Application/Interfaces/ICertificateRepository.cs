using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface ICertificateRepository
    {
        Task<Certificate> New(Certificate certificate);

        Task<Certificate> GetCertificate(Guid id, bool includeLogs = false);

        Task<Certificate> GetCertificate(long uln, int standardCode);

        Task<Certificate> GetCertificate(long uln, int standardCode, string familyName);

        Task<Certificate> GetCertificate(long uln, int standardCode, string familyName, bool includeLogs);

        Task<Certificate> GetCertificate(string certificateReference);

        Task<Certificate> GetCertificate(string certificateReference, string lastName, DateTime? achievementDate);

        Task<Certificate> GetCertificateByUlnOrgIdLastnameAndStandardCode(long uln, string endpointOrganisationId, string lastName, int standardCode);

        Task<Certificate> GetCertificateByUlnLastname(long uln, string lastName);

        Task<List<Certificate>> GetDraftAndCompletedCertificatesFor(long uln);

        Task<int> GetCertificatesReadyToPrintCount(string[] excludedOverallGrades, string[] includedStatus);

        Task<Guid[]> GetCertificatesReadyToPrint(int numberOfCertificates, string[] excludedOverallGrades, string[] includedStatus);

        Task UpdateCertificatesReadyToPrintInBatch(Guid[] certificateIds, int batchNumber);

        Task<Certificate> Update(Certificate certificate, string username, string action, bool updateLog = true, string reasonForChange = null);

        Task Delete(long uln, int standardCode, string username, string action, bool updateLog = true, string reasonForChange = null, string incidentNumber = null);

        Task<Certificate> UpdateProviderName(Guid id, string providerName);

        Task UpdatePrintStatus(Certificate certificate, int batchNumber, string printStatus, DateTime statusAt, string reasonForChange,
            bool updateCertificate, bool updateCertificateBatchLog);

        Task<List<CertificateLog>> GetCertificateLogsFor(Guid certificateId);

        Task<PaginatedList<CertificateHistoryModel>> GetCertificateHistory(string endPointAssessorOrganisationId, int pageIndex, int pageSize, string searchTerm, string sortColumn, bool sortAscending, List<string> statuses);

        Task<string> GetPreviousProviderName(int providerUkPrn);
        Task<CertificateAddress> GetContactPreviousAddress(string epaOrgId, string employerAccountId);
        Task<bool> CertifciateExistsForUln(long uln);

        Task<Certificate> GetCertificateDeletedByUln(long uln);
    }
}