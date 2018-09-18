using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Certificates
{
    public class CreateBatchCertificateRequestValidator : AbstractValidator<BatchCertificateRequest>
    {
        public CreateBatchCertificateRequestValidator(IStringLocalizer<BatchCertificateRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient)
        {
            Include(new BatchCertificateRequestValidator(localiser, organisationQueryRepository, ilrRepository, certificateRepository, assessmentOrgsApiClient));

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