using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Certificates
{
    public class DeleteBatchCertificateRequestValidator : AbstractValidator<DeleteBatchCertificateRequest>
    {
        public DeleteBatchCertificateRequestValidator(IStringLocalizer<DeleteBatchCertificateRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient)
        {
            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999).WithMessage("The UKPRN should contain exactly 8 numbers");
            RuleFor(m => m.Email).NotEmpty();

            RuleFor(m => m.Uln).InclusiveBetween(1000000000, 9999999999).WithMessage("The apprentice's ULN should contain exactly 10 numbers").DependentRules(() =>
            {
                When(m => m.StandardCode > 0 && !string.IsNullOrEmpty(m.FamilyName), () =>
                {
                    RuleFor(m => m).Custom((m, context) =>
                    {
                        var requestedIlr = ilrRepository.Get(m.Uln, m.StandardCode).GetAwaiter().GetResult();
                        var sumbittingEpao = organisationQueryRepository.GetByUkPrn(m.UkPrn).GetAwaiter().GetResult();

                        if (requestedIlr is null || !string.Equals(requestedIlr.FamilyName, m.FamilyName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            context.AddFailure(new ValidationFailure("Uln", "Cannot find apprentice with the specified Uln, FamilyName & StandardCode"));
                        }
                        else if (sumbittingEpao is null)
                        {
                            context.AddFailure(new ValidationFailure("UkPrn", "Cannot find EPAO for specified UkPrn"));
                        }
                        else
                        {
                            var providedStandards = assessmentOrgsApiClient.FindAllStandardsByOrganisationIdAsync(sumbittingEpao.EndPointAssessorOrganisationId).GetAwaiter().GetResult();

                            if (!providedStandards.Any(s => s.StandardCode == m.StandardCode.ToString()))
                            {
                                context.AddFailure(new ValidationFailure("StandardCode", "EPAO does not provide this Standard"));
                            }
                        }
                    });
                });
            });

            RuleFor(m => m.FamilyName).NotEmpty().WithMessage("Enter the apprentice's last name");
            RuleFor(m => m.StandardCode).GreaterThan(0).NotEmpty().WithMessage("A standard should be selected");
            RuleFor(m => m.CertificateReference).NotEmpty().WithMessage("Enter the certificate reference").DependentRules(() =>
            {
                RuleFor(m => m).Custom((m, context) =>
                {
                    var existingCertificate = certificateRepository.GetCertificate(m.Uln, m.StandardCode).GetAwaiter().GetResult();
                    var sumbittingEpao = organisationQueryRepository.GetByUkPrn(m.UkPrn).GetAwaiter().GetResult();

                    if (existingCertificate is null || !string.Equals(existingCertificate.CertificateReference, m.CertificateReference, StringComparison.InvariantCultureIgnoreCase))
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate not found"));
                    }
                    else if (sumbittingEpao?.Id != existingCertificate.OrganisationId)
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"EPAO is not the creator of this Certificate"));
                    }
                    else if (existingCertificate.Status == CertificateStatus.Submitted || existingCertificate.Status == CertificateStatus.Printed
                            || existingCertificate.Status == CertificateStatus.Reprint)
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate cannot be Deleted when in '{CertificateStatus.Submitted}' status"));
                    }
                });
            });
        }
    }
}
