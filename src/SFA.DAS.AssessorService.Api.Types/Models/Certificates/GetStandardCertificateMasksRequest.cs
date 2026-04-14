using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetStandardCertificateMasksRequest : IRequest<GetStandardCertificateMasksResponse>
    {
        public long[] Exclude { get; set; }
    }
}
