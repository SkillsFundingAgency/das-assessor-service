using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface ICertificateRepository
    {
        Task<Certificate> New(Certificate certificate);
        Task<Certificate> GetCertificate(Guid id);
        Task<Certificate> GetCertificate(long uln, int standardCode);
        Task<List<Certificate>> GetCompletedCertificatesFor(long uln);
        Task<List<Certificate>> GetCertificates(string requestStatus);
        Task<Certificate> Update(Certificate certificate, string username, string action);
        Task UpdateStatuses(UpdateCertificatesBatchToIndicatePrintedRequest updateCertificatesBatchToIndicatePrintedRequest);
        Task<List<CertificateLog>> GetCertificateLogsFor(Guid certificateId);
        Task<bool> RequestReprint(string userName, string certificateReference, string lastName, DateTime? achievementDate);
    }
}