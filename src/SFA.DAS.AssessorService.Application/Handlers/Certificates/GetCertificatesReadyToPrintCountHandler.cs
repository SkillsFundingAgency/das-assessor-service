using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesReadyToPrintCountHandler : IRequestHandler<GetCertificatesReadyToPrintCountRequest, int>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<GetCertificatesReadyToPrintCountHandler> _logger;

        public GetCertificatesReadyToPrintCountHandler(ICertificateRepository certificateRepository,
            ILogger<GetCertificatesReadyToPrintCountHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<int> Handle(GetCertificatesReadyToPrintCountRequest request, CancellationToken cancellationToken)
        {
            var excludedOverallGrades = new[] { "Fail" };
            var includedStatus = new[] { "Submitted", "Reprint" };

            return await _certificateRepository.GetCertificatesReadyToPrintCount(excludedOverallGrades, includedStatus);
        }
    }
}