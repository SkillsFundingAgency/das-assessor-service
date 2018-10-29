using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class PrivateCertificatePrepareForBatchRunHandler : IRequestHandler<PrivateCertificatePrepareForBatchRunRequest>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateCertificateHandler> _logger;

        public PrivateCertificatePrepareForBatchRunHandler(ICertificateRepository certificateRepository,
            ILogger<UpdateCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task Handle(PrivateCertificatePrepareForBatchRunRequest request, CancellationToken cancellationToken)
        {
            var statuses = new List<string>
            {
                Domain.Consts.CertificateStatus.Approved,
                Domain.Consts.CertificateStatus.Rejected
            };

            var certificates = await _certificateRepository.GetCertificates(statuses);
            foreach (var certificate in certificates)
            {
                if (certificate.Status == Domain.Consts.CertificateStatus.Approved)
                {
                    certificate.Status = Domain.Consts.CertificateStatus.Submitted;
                }

                if (certificate.Status == Domain.Consts.CertificateStatus.Rejected)
                {
                    certificate.Status = Domain.Consts.CertificateStatus.Draft;
                }

                await _certificateRepository.Update(certificate, request.UserName, CertificateActions.ApprovePrivateCertificate);
            }
        }
    }
}