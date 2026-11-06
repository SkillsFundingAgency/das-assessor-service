using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Validators;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.Helpers;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class CertificateReprintCommandHandler : IRequestHandler<CertificateReprintCommand, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IValidator<PrintDeliveryAddress> _validator;

        public CertificateReprintCommandHandler(ICertificateRepository certificateRepository, IDateTimeHelper dateTimeHelper, IValidator<PrintDeliveryAddress> validator)
        {
            _certificateRepository = certificateRepository;
            _dateTimeHelper = dateTimeHelper;
            _validator = validator;
        }

        public async Task<Certificate> Handle(CertificateReprintCommand request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate<Certificate>(request.CertificateId);
            if (certificate == null)
                throw new NotFoundException();

            if (certificate.Status != CertificateStatus.Reprint && CertificateStatus.CanRequestReprintCertificate(certificate.Status))
            {
                var data = certificate.CertificateData;

                var address = new PrintDeliveryAddress(
                    data?.SendTo ?? CertificateSendTo.None,
                    data?.ContactName,
                    data?.ContactOrganisation,
                    data?.ContactAddLine1,
                    data?.ContactPostCode);

                var validation = _validator.Validate(address);

                if (!validation.IsValid)
                {
                    var message = string.Join("; ", validation.Errors.Select(error => error.ErrorMessage));
                    throw new BadRequestException(message);
                }

                await _certificateRepository.UpdateReprintRequested<Certificate>(certificate.Id, _dateTimeHelper.DateTimeUtcNow, request.Username);
            }

            return certificate;
        }
    }
}