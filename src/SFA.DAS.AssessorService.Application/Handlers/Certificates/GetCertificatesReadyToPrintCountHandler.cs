using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Handlers.Certificates
{
    public class GetCertificatesReadyToPrintCountHandler : IRequestHandler<GetCertificatesReadyToPrintCountRequest, int>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetCertificatesReadyToPrintCountHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<int> Handle(GetCertificatesReadyToPrintCountRequest request, CancellationToken cancellationToken)
        {
            var excludedOverallGrades = new[] { "Fail" };
            var includedStatus = new[]
            {
                CertificateStatus.PrintRequested,
                CertificateStatus.Reprint
            };

            return await _certificateRepository.GetCertificatesReadyToPrintCount(excludedOverallGrades, includedStatus);
        }
    }
}