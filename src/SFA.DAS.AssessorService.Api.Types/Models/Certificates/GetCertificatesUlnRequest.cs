using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificatesUlnRequest : IRequest<GetCertificatesUlnResponse>
    {
        public long Uln { get; set; }
    }
}