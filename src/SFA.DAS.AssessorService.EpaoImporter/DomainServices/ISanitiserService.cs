using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.EpaoImporter.DomainServices
{
    public interface ISanitiserService
    {
        List<CertificateResponse> Sanitize(List<CertificateResponse> certificateResponses);
    }
}