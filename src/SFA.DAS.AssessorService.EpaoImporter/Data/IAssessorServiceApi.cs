using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.EpaoImporter.Data
{
    public interface IAssessorServiceApi
    {
        Task<int> GenerateBatchNumber();
        Task<IEnumerable<CertificateResponse>> GetCertificatesToBePrinted();
        Task ChangeStatusToPrinted(int batchNumber, IEnumerable<CertificateResponse> responses);
        Task<EMailTemplate> GetEmailTemplate();
    }
}