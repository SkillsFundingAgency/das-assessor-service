using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Epas
{
    public class CreateBatchEpaRequestValidator : AbstractValidator<CreateBatchEpaRequest>
    {
        public CreateBatchEpaRequestValidator(IStringLocalizer<BatchEpaRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, ILearnerRepository learnerRepository, ICertificateRepository certificateRepository, IStandardService standardService)
        {
            Include(new BatchEpaRequestValidator(localiser, organisationQueryRepository, learnerRepository, standardService));

            RuleFor(m => m.EpaDetails.EpaReference).Empty().WithMessage("EPA reference must be empty").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    var existingCertificate = await certificateRepository.GetCertificate(m.Uln, m.StandardCode);

                    if (existingCertificate != null)
                    {
                        var submittedCertificate = !(existingCertificate.Status == CertificateStatus.Draft || existingCertificate.Status == CertificateStatus.Deleted);
                        var outcomeIsAFail = existingCertificate.CertificateData.OverallGrade == CertificateGrade.Fail;
                        var outcomeIsAPass = !outcomeIsAFail;
                        var isDraftCertificate = existingCertificate.Status == CertificateStatus.Draft;
                        var canUpdateDraftCertificate = string.IsNullOrEmpty(existingCertificate.CertificateData.EpaDetails?.LatestEpaOutcome);

                        if (submittedCertificate && outcomeIsAPass)
                        {
                            context.AddFailure(new ValidationFailure("EpaDetails", $"Certificate already exists, cannot create EPA record"));
                        }
                        else if (submittedCertificate && outcomeIsAFail)
                        {
                            context.AddFailure(new ValidationFailure("EpaDetails", $"EPA already provided for the learner"));
                        }
                        else if (isDraftCertificate && !canUpdateDraftCertificate)
                        {
                            context.AddFailure(new ValidationFailure("EpaDetails", $"EPA already provided for the learner"));
                        }
                    }
                });
            });
        }
    }
}
