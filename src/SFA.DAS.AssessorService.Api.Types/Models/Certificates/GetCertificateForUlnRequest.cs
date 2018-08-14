using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificateForUlnRequest : IRequest<Certificate>
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
    }
}