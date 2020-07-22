using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesToBePrintedHandler : IRequestHandler<GetCertificatesToBePrintedRequest, CertificatesToBePrintedResponse>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<GetCertificatesHistoryHandler> _logger;

        public GetCertificatesToBePrintedHandler(ICertificateRepository certificateRepository, ILogger<GetCertificatesHistoryHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<CertificatesToBePrintedResponse> Handle(GetCertificatesToBePrintedRequest request, CancellationToken cancellationToken)
        {
            var toBePrintedStatus = new List<string>
            {
                Domain.Consts.CertificateStatus.Submitted,
                Domain.Consts.CertificateStatus.Reprint
            };

            var certificates = await _certificateRepository.GetCertificatesToBePrinted(toBePrintedStatus);
            
            var certificatesToBePrintedResponse = new CertificatesToBePrintedResponse()
            {
                Certificates = certificates
                    .Where(p => !string.IsNullOrEmpty(p.OverallGrade) && p.OverallGrade != CertificateGrade.Fail)
                    .ToList()
            };

            return certificatesToBePrintedResponse;
        }
    }
}