﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificateStatusHandler : IRequestHandler<UpdateCertificateStatusRequest>
    {
        private readonly ICertificateRepository _certificateRepository;

        public UpdateCertificateStatusHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task Handle(UpdateCertificateStatusRequest request, CancellationToken cancellationToken)
        {
            await _certificateRepository.UpdateStatus(request.CertificateReference, request.Status);
        }
    }
}