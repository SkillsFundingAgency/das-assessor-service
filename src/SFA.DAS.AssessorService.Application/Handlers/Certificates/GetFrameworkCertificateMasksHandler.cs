using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetFrameworkCertificateMasksHandler : IRequestHandler<GetFrameworkCertificateMasksRequest, GetFrameworkCertificateMasksResponse>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetFrameworkCertificateMasksHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<GetFrameworkCertificateMasksResponse> Handle(GetFrameworkCertificateMasksRequest request, CancellationToken cancellationToken)
        {
            var masks = await _certificateRepository.GetFrameworkMasks(request.Exclude);
                return new GetFrameworkCertificateMasksResponse { Masks = masks.Select(m => new CertificateMask
                {
                    CertificateType = m.CertificateType,
                    CourseCode = m.CourseCode,
                    CourseName = m.CourseName,
                    CourseLevel = m.CourseLevel,
                    ProviderName = m.ProviderName
                }).ToList() };
        }
    }
}
