using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.Services;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Certificates
{
    public class CreateBatchCertificateRequestValidator : AbstractValidator<BatchCertificateRequest>
    {
        public CreateBatchCertificateRequestValidator(IStringLocalizer<BatchCertificateRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient, IStandardService standardService)
        {
            Include(new BatchCertificateRequestValidator(localiser, organisationQueryRepository, ilrRepository, certificateRepository, assessmentOrgsApiClient, standardService));

            RuleFor(m => m.CertificateReference).Empty().WithMessage("Certificate reference must be empty").DependentRules(() =>
            {
                RuleFor(m => m).Custom((m, context) =>
                {
                    var existingCertificate = certificateRepository.GetCertificate(m.Uln, m.StandardCode).Result;

                    if (existingCertificate != null && existingCertificate.Status != CertificateStatus.Deleted)
                    {
                        context.AddFailure(new ValidationFailure("CertificateData", $"Certificate already exists: {existingCertificate.CertificateReference}"));
                    }
                });
            });
        }
    }
}