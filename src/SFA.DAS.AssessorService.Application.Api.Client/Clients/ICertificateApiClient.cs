using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface ICertificateApiClient
    {
        Task<Certificate> Start(StartCertificateRequest request);
        Task<Certificate> GetCertificate(Guid certificateId);
        Task<Certificate> UpdateCertificate(UpdateCertificateRequest updateGradeRequest);
        Task<PaginatedList<CertificateHistoryResponse>> GetCertificateHistory(int pageIndex, string userName);
    }
}