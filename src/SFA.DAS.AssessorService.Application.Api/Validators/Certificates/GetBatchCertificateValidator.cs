using System;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Certificates
{
    public class GetBatchCertificateRequestValidator : AbstractValidator<GetBatchCertificateRequest>
    {
        public GetBatchCertificateRequestValidator(IStringLocalizer<GetBatchCertificateRequestValidator> localiser, ICertificateRepository certificateRepository)
        {
            RuleFor(m => m.Uln).InclusiveBetween(1000000000, 9999999999).WithMessage("The apprentice's ULN should contain exactly 10 numbers");
            RuleFor(m => m.FamilyName).NotEmpty().WithMessage("Enter the apprentice's last name");
            RuleFor(m => m.StandardCode).GreaterThan(0).WithMessage("A standard should be selected");
            RuleFor(m => m.CertificateReference).NotEmpty().WithMessage("Enter the certificate reference");
            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999).WithMessage("The UKPRN should contain exactly 8 numbers");
            RuleFor(m => m.Email).NotEmpty();

            RuleFor(m => m)
                .Custom((m, context) =>
                {
                    var existingCertificate = certificateRepository.GetCertificate(m.Uln, m.StandardCode).GetAwaiter().GetResult();

                    if (existingCertificate is null)
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate not found"));
                    }
                    else if (!existingCertificate.CertificateReference.Equals(m.CertificateReference, StringComparison.InvariantCultureIgnoreCase))
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Invalid certificate reference"));
                    }
                    else if (!existingCertificate.CertificateData.Contains(m.FamilyName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        context.AddFailure(new ValidationFailure("FamilyName", $"Invalid family name"));
                    }
                });
        }
    }
}
