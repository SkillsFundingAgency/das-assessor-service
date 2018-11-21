using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using System;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Certificates
{
    public class UpdateBatchCertificateRequestValidator : AbstractValidator<BatchCertificateRequest>
    {
        public UpdateBatchCertificateRequestValidator(IStringLocalizer<BatchCertificateRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient)
        {
            Include(new BatchCertificateRequestValidator(localiser, organisationQueryRepository, ilrRepository, certificateRepository, assessmentOrgsApiClient));

            RuleFor(m => m.CertificateReference).NotEmpty().WithMessage("Enter the certificate reference").DependentRules(() =>
            {
                RuleFor(m => m).Custom((m, context) =>
                {
                    var existingCertificate = certificateRepository.GetCertificate(m.Uln, m.StandardCode).GetAwaiter().GetResult();
                    var sumbittingEpao = organisationQueryRepository.GetByUkPrn(m.UkPrn).GetAwaiter().GetResult();

                    if (existingCertificate is null || !string.Equals(existingCertificate.CertificateReference, m.CertificateReference, StringComparison.InvariantCultureIgnoreCase)
                        || existingCertificate.Status == CertificateStatus.Deleted)
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate not found"));
                    }
                    else if (sumbittingEpao?.Id != existingCertificate.OrganisationId)
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"EPAO is not the creator of this Certificate"));
                    }
                    else if (existingCertificate.Status != CertificateStatus.Draft)
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate is not in '{CertificateStatus.Draft}' status"));
                    }
                });
            });
        }
    }
}
