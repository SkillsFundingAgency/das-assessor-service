using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetStandardCertificateMasksHandler : IRequestHandler<GetStandardCertificateMasksRequest, GetStandardCertificateMasksResponse>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetStandardCertificateMasksHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<GetStandardCertificateMasksResponse> Handle(GetStandardCertificateMasksRequest request, CancellationToken cancellationToken)
        {
            var masks = await _certificateRepository.GetStandardMasks(request.Exclude);
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
