using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetFrameworkCertificateMasksHandler : IRequestHandler<GetFrameworkCertificateMasksRequest, GetFrameworkCertificateMasksResponse>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IApiConfiguration _config;

        public GetFrameworkCertificateMasksHandler(ICertificateRepository certificateRepository, IApiConfiguration config)
        {
            _certificateRepository = certificateRepository;
            _config = config;
        }

        public async Task<GetFrameworkCertificateMasksResponse> Handle(GetFrameworkCertificateMasksRequest request, CancellationToken cancellationToken)
        {
            var masks = await _certificateRepository.GetFrameworkMasks(request.Exclude, _config.MasksMaxCount);
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
