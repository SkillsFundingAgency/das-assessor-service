using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Epas
{
    public class CreateBatchEpaRequestValidator : AbstractValidator<BatchEpaRequest>
    {
        public CreateBatchEpaRequestValidator(IStringLocalizer<BatchEpaRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IStandardService standardService)
        {
            Include(new BatchEpaRequestValidator(localiser, organisationQueryRepository, ilrRepository, standardService));

            RuleFor(m => m.EpaDetails.EpaReference).Empty().WithMessage("EPA reference must be empty").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    var existingCertificate = await certificateRepository.GetCertificate(m.Uln, m.StandardCode);

                    if (existingCertificate != null)
                    {
                        switch (existingCertificate.Status)
                        {
                            case CertificateStatus.Deleted:
                                break;
                            case CertificateStatus.Draft:
                                var certData = JsonConvert.DeserializeObject<CertificateData>(existingCertificate.CertificateData);

                                if (!string.IsNullOrEmpty(certData.OverallGrade) && certData.AchievementDate.HasValue && !string.IsNullOrEmpty(certData.ContactPostCode))
                                {
                                    context.AddFailure(new ValidationFailure("EpaDetails", $"Certificate already exists, cannot create EPA record"));
                                }
                                else if (!string.IsNullOrEmpty(certData.EpaDetails?.LatestEpaOutcome))
                                {
                                    context.AddFailure(new ValidationFailure("EpaDetails", $"EPA already provided for the learner"));
                                }
                                break;
                            default:
                                context.AddFailure(new ValidationFailure("EpaDetails", $"Certificate already exists, cannot create EPA record"));
                                break;
                        }
                    }
                });
            });
        }
    }
}
