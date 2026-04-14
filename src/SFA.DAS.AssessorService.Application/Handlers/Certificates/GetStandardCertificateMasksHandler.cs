using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetStandardCertificateMasksHandler : IRequestHandler<GetStandardCertificateMasksRequest, GetStandardCertificateMasksResponse>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IApiConfiguration _config;

        public GetStandardCertificateMasksHandler(ICertificateRepository certificateRepository, IApiConfiguration config)
        {
            _certificateRepository = certificateRepository;
            _config = config;
        }

        public async Task<GetStandardCertificateMasksResponse> Handle(GetStandardCertificateMasksRequest request, CancellationToken cancellationToken)
        {
            var masks = await _certificateRepository.GetStandardMasks(request.Exclude, _config.MasksMaxCount);
            return new GetStandardCertificateMasksResponse { Masks = masks.Select(m => new CertificateMask
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
