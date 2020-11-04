using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificatesPrintStatusUpdateRequest : IRequest<ValidationResponse>
    {
        public List<CertificatePrintStatusUpdate> CertificatePrintStatusUpdates { get; set; }
    }
}