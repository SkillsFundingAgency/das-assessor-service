using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Standards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.Infrastructure
{
    public interface IApiClient
    {
        Task<GetLearnerResponse> GetLearner(GetBatchLearnerRequest request);

        Task<IEnumerable<CreateEpaResponse>> CreateEpas(IEnumerable<CreateBatchEpaRequest> request);
        Task<IEnumerable<UpdateEpaResponse>> UpdateEpas(IEnumerable<UpdateBatchEpaRequest> request);
        Task<ApiResponse> DeleteEpa(DeleteBatchEpaRequest request);

        Task<GetCertificateResponse> GetCertificate(GetBatchCertificateRequest request);
        Task<IEnumerable<CreateCertificateResponse>> CreateCertificates(IEnumerable<CreateBatchCertificateRequest> request);
        Task<IEnumerable<UpdateCertificateResponse>> UpdateCertificates(IEnumerable<UpdateBatchCertificateRequest> request);
        Task<IEnumerable<SubmitCertificateResponse>> SubmitCertificates(IEnumerable<SubmitBatchCertificateRequest> request);
        Task<ApiResponse> DeleteCertificate(DeleteBatchCertificateRequest request);

        Task<IEnumerable<StandardOptions>> GetStandardOptionsList();
        Task<StandardOptions> GetStandardOptionsByStandard(string standard);
        Task<StandardOptions> GetStandardOptionsByStandardReferenceAndVersion(string standardReference, decimal version);
    }
}