using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.EpaoImporter.Interfaces
{
    public interface IPrintingSpreadsheetCreator
    {
        void Create(int batchNumber, IEnumerable<CertificateResponse> certificates);
    }
}
