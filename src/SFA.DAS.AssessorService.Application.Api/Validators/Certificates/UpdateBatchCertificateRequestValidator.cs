using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Certificates
{
    public class UpdateBatchCertificateRequestValidator : AbstractValidator<BatchCertificateRequest>
    {
        public UpdateBatchCertificateRequestValidator(IStringLocalizer<BatchCertificateRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient)
        {
            Include(new BatchCertificateRequestValidator(localiser, organisationQueryRepository, ilrRepository, certificateRepository, assessmentOrgsApiClient));

            RuleFor(m => m.CertificateReference).NotEmpty();

            RuleFor(m => m)
                .Custom((m, context) =>
                {
                    var existingCertificate = certificateRepository.GetCertificate(m.Uln, m.StandardCode).Result;

                    if (existingCertificate == null || !string.Equals(existingCertificate.CertificateReference, m.CertificateReference))
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate not found"));
                    }
                    else if(existingCertificate.Status != CertificateStatus.Draft && existingCertificate.Status != CertificateStatus.Deleted) // TODO: Unit Test
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate is not in '{CertificateStatus.Draft}' status"));
                    }
                });
        }
    }
}
