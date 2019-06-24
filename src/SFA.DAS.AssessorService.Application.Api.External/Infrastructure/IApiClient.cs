using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Standards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.Infrastructure
{
    public interface IApiClient
    {
        Task<GetCertificateResponse> GetCertificate(GetBatchCertificateRequest request);
        Task<IEnumerable<CreateCertificateResponse>> CreateCertificates(IEnumerable<CreateBatchCertificateRequest> request);
        Task<IEnumerable<UpdateCertificateResponse>> UpdateCertificates(IEnumerable<UpdateBatchCertificateRequest> request);
        Task<IEnumerable<SubmitCertificateResponse>> SubmitCertificates(IEnumerable<SubmitBatchCertificateRequest> request);
        Task<ApiResponse> DeleteCertificate(DeleteBatchCertificateRequest request);

        Task<IEnumerable<StandardOptions>> GetStandards();
        Task<StandardOptions> GetStandard(string standard);
    }
}