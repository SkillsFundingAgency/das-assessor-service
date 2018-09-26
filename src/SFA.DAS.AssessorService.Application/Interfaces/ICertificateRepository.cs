using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface ICertificateRepository
    {
        Task<Certificate> New(Certificate certificate);
        Task<Certificate> NewPrivate(Certificate certificate, string endpointOrganisationId);
        Task<Certificate> GetCertificate(Guid id);
        Task<Certificate> GetCertificate(long uln, int standardCode);
        Task<Certificate> GetPrivateCertificate(long uln,string endpointOrganisationId,string lastName);
        Task<Certificate> GetCertificate(string certificateReference, string lastName, DateTime? achievementDate);
        Task<List<Certificate>> GetCompletedCertificatesFor(long uln);
        Task<List<Certificate>> GetCertificates(List<string> statuses);
        Task<Certificate> Update(Certificate certificate, string username, string action, bool updateLog = true);
        Task<Certificate> UpdateProviderName(Guid id, string providerName);
        Task UpdateStatuses(UpdateCertificatesBatchToIndicatePrintedRequest updateCertificatesBatchToIndicatePrintedRequest);
        Task<List<CertificateLog>> GetCertificateLogsFor(Guid certificateId);
        Task<PaginatedList<Certificate>> GetCertificateHistory(string userName, int pageIndex, int pageSize);
        Task<string> GetPreviousProviderName(int providerUkPrn);
        Task<CertificateAddress> GetContactPreviousAddress(string userName);
        Task<List<Option>> GetOptions(int stdCode);
    }
}