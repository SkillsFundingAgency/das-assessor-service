using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.EpaoImporter.Interfaces
{
    public interface IPrintingJsonCreator
    {
        void Create(int batchNumber, List<CertificateResponse> certificates, string certificatesFileName);
    }
}
