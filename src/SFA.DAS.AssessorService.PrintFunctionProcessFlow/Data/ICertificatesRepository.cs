using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data
{
    public interface ICertificatesRepository
    {
        Task<int> GenerateBatchNumber();
        Task<IEnumerable<CertificateResponse>> GetCertificatesToBePrinted();
        Task ChangeStatusToPrinted(string batchNumber, IEnumerable<CertificateResponse> responses);
    }
}