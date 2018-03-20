using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class StartCertificateRequest : IRequest<Certificate>
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public int UkPrn { get; set; }
        public string Username { get; set; }
    }
}   