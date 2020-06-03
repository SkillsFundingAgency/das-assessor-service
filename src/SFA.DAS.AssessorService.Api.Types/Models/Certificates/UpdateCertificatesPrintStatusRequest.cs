using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class UpdateCertificatesPrintStatusRequest : IRequest<ValidationResponse>
    {
        public List<CertificatePrintStatus> CertificatePrintStatuses { get; set; }
    }
}