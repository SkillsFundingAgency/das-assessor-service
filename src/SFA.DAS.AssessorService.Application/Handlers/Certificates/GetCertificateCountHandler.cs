using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;

using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using System.Text;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesCountHandler : IRequestHandler<GetCertificatesCountRequest, CertificatesCountResponse>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<GetCertificatesHistoryHandler> _logger;

        public GetCertificatesCountHandler(ICertificateRepository certificateRepository,
            ILogger<GetCertificatesHistoryHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<CertificatesCountResponse> Handle(GetCertificatesCountRequest request, CancellationToken cancellationToken)
        {
            const int pageSize = 10;
            var statuses = new List<string>
            {
                Domain.Consts.CertificateStatus.Submitted,
                Domain.Consts.CertificateStatus.Printed,
                Domain.Consts.CertificateStatus.Reprint
            };
            
            return new CertificatesCountResponse(
                 await _certificateRepository.GetCertificatesCount(request.Username, statuses));
        }
    }
}
