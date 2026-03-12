using System;
using FluentValidation;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class UpdateCertificatePrintRequestValidator : AbstractValidator<UpdateCertificatePrintRequestCommand>
    {
        public UpdateCertificatePrintRequestValidator()
        {
            RuleFor(r => r.CertificateId).NotEmpty().WithMessage("CertificateId must not be empty");
            RuleFor(r => r.PrintRequestedAt).NotEmpty().WithMessage("PrintRequestedAt must not be empty");
            RuleFor(r => r.PrintRequestedBy).NotEmpty().WithMessage("PrintRequestedBy must not be empty");
            RuleFor(r => r.Address).NotNull().WithMessage("Address must be supplied");

            When(r => r.Address != null, () =>
            {
                RuleFor(r => r.Address.ContactName).NotEmpty().WithMessage("Address.ContactName must not be empty");
                RuleFor(r => r.Address.ContactPostCode).NotEmpty().WithMessage("Address.ContactPostCode must not be empty");
            });
        }
    }
}
