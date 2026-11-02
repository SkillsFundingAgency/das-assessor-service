using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.Helpers;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificateRequestReprintCommandHandler : IRequestHandler<UpdateCertificateRequestReprintCommand, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IDateTimeHelper _dateTimeHelper;

        public UpdateCertificateRequestReprintCommandHandler(ICertificateRepository certificateRepository, IDateTimeHelper dateTimeHelper)
        {
            _certificateRepository = certificateRepository;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<Certificate> Handle(UpdateCertificateRequestReprintCommand request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate<Certificate>(request.CertificateId);
            if (certificate == null)
                throw new NotFoundException();

            if (certificate.Status != CertificateStatus.Reprint && CertificateStatus.CanRequestReprintCertificate(certificate.Status))
            {
                await _certificateRepository.UpdateReprintRequested(certificate.Id, _dateTimeHelper.DateTimeUtcNow, request.Username);
            }

            return certificate;
        }
    }
}