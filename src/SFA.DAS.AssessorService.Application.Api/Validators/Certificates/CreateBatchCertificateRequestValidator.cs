using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Certificates
{
    public class CreateBatchCertificateRequestValidator : AbstractValidator<BatchCertificateRequest>
    {
        public CreateBatchCertificateRequestValidator(IStringLocalizer<BatchCertificateRequestValidator> localiser, ICertificateRepository certificateRepository)
        {
            Include(new BatchCertificateRequestValidator(localiser));

            RuleFor(m => m.CertificateReference).Empty();

            RuleFor(m => m)
                .Custom((m, context) =>
                {
                    var existingCertificate = certificateRepository.GetCertificate(m.Uln, m.StandardCode).Result;

                    if (existingCertificate != null)
                    {
                        context.AddFailure(new ValidationFailure("CertificateData", $"Certificate already exists: {existingCertificate.CertificateReference}"));
                    }
                });
        }
    }
}