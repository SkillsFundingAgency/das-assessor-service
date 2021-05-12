using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Epas
{
    public class UpdateBatchEpaRequestValidator : AbstractValidator<BatchEpaRequest>
    {
        public UpdateBatchEpaRequestValidator(IStringLocalizer<BatchEpaRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IStandardService standardService)
        {
            Include(new BatchEpaRequestValidator(localiser, organisationQueryRepository, ilrRepository, standardService));

            RuleFor(m => m.EpaDetails.EpaReference).NotEmpty().WithMessage("Provide EPA reference").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    var existingCertificate = await certificateRepository.GetCertificate(m.Uln, m.StandardCode);
                    var sumbittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);

                    if (existingCertificate is null || !string.Equals(existingCertificate.CertificateReference, m.EpaDetails.EpaReference, StringComparison.InvariantCultureIgnoreCase))
                    {
                        context.AddFailure(new ValidationFailure("EpaReference", $"EPA not found"));
                    }
                    else if (sumbittingEpao?.Id != existingCertificate.OrganisationId)
                    {
                        context.AddFailure(new ValidationFailure("EpaReference", $"Your organisation is not the creator of this EPA"));
                    }
                    else
                    {
                        switch (existingCertificate.Status)
                        {
                            case CertificateStatus.Deleted:
                                context.AddFailure(new ValidationFailure("EpaReference", $"EPA not found"));
                                break;
                            case CertificateStatus.Draft:
                                var certData = JsonConvert.DeserializeObject<CertificateData>(existingCertificate.CertificateData);

                                if (!string.IsNullOrEmpty(certData.OverallGrade) && certData.AchievementDate.HasValue && !string.IsNullOrEmpty(certData.ContactPostCode))
                                {
                                    context.AddFailure(new ValidationFailure("EpaReference", $"Certificate already exists, cannot update EPA record"));
                                }
                                break;
                            default:
                                context.AddFailure(new ValidationFailure("EpaReference", $"Certificate already exists, cannot update EPA record"));
                                break;
                        }
                    }
                });
            });
        }
    }
}
