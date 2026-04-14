using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetFrameworkCertificateMasksRequest : IRequest<GetFrameworkCertificateMasksResponse>
    {
        public long[] Exclude { get; set; }
    }
}
