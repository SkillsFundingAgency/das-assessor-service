using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Certificates
{
    public class DeleteBatchCertificateRequestValidator : AbstractValidator<DeleteBatchCertificateRequest>
    {
        public DeleteBatchCertificateRequestValidator(IStringLocalizer<DeleteBatchCertificateRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient)
        {
            RuleFor(m => m.Uln).InclusiveBetween(1000000000, 9999999999).WithMessage("The apprentice's ULN should contain exactly 10 numbers");
            RuleFor(m => m.FamilyName).NotEmpty().WithMessage("Enter the apprentice's last name");
            RuleFor(m => m.StandardCode).NotEmpty().WithMessage("A standard should be selected");
            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999).WithMessage("The UKPRN should contain exactly 8 numbers");
            RuleFor(m => m.Username).NotEmpty();

            RuleFor(m => m)
                .Custom((m, context) =>
                {
                    var existingCertificate = certificateRepository.GetCertificate(m.Uln, m.StandardCode).Result;

                    if (existingCertificate == null)
                    {
                        context.AddFailure(new ValidationFailure("Certificate", $"Certificate not found"));
                    }
                    else if (existingCertificate.Status == CertificateStatus.Submitted)
                    {
                        context.AddFailure(new ValidationFailure("Certificate", $"Certificate cannot be Deleted when in '{CertificateStatus.Submitted}' status"));
                    }
                    else if (existingCertificate.Status == CertificateStatus.Printed)
                    {
                        context.AddFailure(new ValidationFailure("Certificate", $"Certificate cannot be Deleted when in '{CertificateStatus.Printed}' status"));
                    }
                });

            RuleFor(m => m)
                .Custom((m, context) =>
                {
                    var requestedIlr = ilrRepository.Get(m.Uln, m.StandardCode).GetAwaiter().GetResult();
                    var sumbittingEpao = organisationQueryRepository.GetByUkPrn(m.UkPrn).GetAwaiter().GetResult();

                    if (requestedIlr == null || !string.Equals(requestedIlr.FamilyName, m.FamilyName))
                    {
                        context.AddFailure(new ValidationFailure("Uln", "Cannot find entry for specified Uln, FamilyName & StandardCode"));
                    }
                    else if (sumbittingEpao == null)
                    {
                        context.AddFailure(new ValidationFailure("UkPrn", "Cannot find EPAO for specified UkPrn"));
                    }
                    else
                    {
                        var providedStandards = assessmentOrgsApiClient.FindAllStandardsByOrganisationIdAsync(sumbittingEpao.EndPointAssessorOrganisationId).GetAwaiter().GetResult();

                        if (!providedStandards.Where(s => s.StandardCode == m.StandardCode.ToString()).Any())
                        {
                            context.AddFailure(new ValidationFailure("StandardCode", "EPAO does not provide this Standard"));
                        }
                    }
                });
        }
    }
}
