using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Api.External.Messages;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;

namespace SFA.DAS.AssessorService.Application.Api.External.Infrastructure
{
    public interface IApiClient
    {
        Task<IEnumerable<BatchCertificateResponse>> CreateCertificates(IEnumerable<BatchCertificateRequest> request);
        Task<ApiResponse> DeleteCertificate(DeleteCertificateRequest request);
        Task<GetCertificateResponse> GetCertificate(GetCertificateRequest request);
        Task<IEnumerable<SubmitBatchCertificateResponse>> SubmitCertificates(IEnumerable<SubmitBatchCertificateRequest> request);
        Task<IEnumerable<BatchCertificateResponse>> UpdateCertificates(IEnumerable<BatchCertificateRequest> request);
    }
}