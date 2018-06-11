using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class RequestReprintHandler : IRequestHandler<CertificateReprintRequest>
    {
        private readonly ICertificateRepository _certificateRepository;

        public RequestReprintHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task Handle(CertificateReprintRequest request, CancellationToken cancellationToken)
        {
            var certificate  = await _certificateRepository.GetCertificate(request.CertificateReference, request.LastName, request.AchievementDate);
            if (certificate == null)
                throw new NotFound();

            if (certificate.Status == SFA.DAS.AssessorService.Domain.Consts.CertificateStatus.Printed)
            {
                certificate.Status = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus.Reprint;
                await _certificateRepository.Update(certificate, request.Username,
                    SFA.DAS.AssessorService.Domain.Consts.CertificateActions.Reprint);
            }
            else
            {
                throw new NotFound();
            }
        }
    }
}