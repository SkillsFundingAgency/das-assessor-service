using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificatePrintStatusUpdateRequest : CertificatePrintStatusUpdate, IRequest<ValidationResponse>
    {
    }
}