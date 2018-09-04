using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface ICertificateApiClient
    {
        Task<Certificate> Start(StartCertificateRequest request);
        Task<Certificate> StartPrivate(StartCertificatePrivateRequest request);
        Task<Certificate> GetCertificate(Guid certificateId);
        Task<Certificate> UpdateCertificate(UpdateCertificateRequest updateGradeRequest);
        Task<PaginatedList<CertificateHistoryResponse>> GetCertificateHistory(int pageIndex, string userName);
        Task<CertificateAddress> GetContactPreviousAddress(string userName);
        Task<List<CertificateAddress>> GetPreviousAddressess(string userName);
    }
}