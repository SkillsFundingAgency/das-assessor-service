using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.EpaoImporter.Data;

namespace SFA.DAS.AssessorService.EpaoImporter.Interfaces
{
    public interface ICoverLetterService
    {
        Task<CoverLettersProduced> Create(int batchNumber, IEnumerable<CertificateResponse> certificates);
    }
}