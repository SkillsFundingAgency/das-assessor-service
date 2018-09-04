using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Certificates
{
    public class BatchCertificateRequestValidator : AbstractValidator<BatchCertificateRequest>
    {
        public BatchCertificateRequestValidator(IStringLocalizer<BatchCertificateRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient)
        {
            RuleFor(m => m.Uln).InclusiveBetween(1000000000, 9999999999).WithMessage("The apprentice's ULN should contain exactly 10 numbers");
            RuleFor(m => m.FamilyName).NotEmpty().WithMessage("Enter the apprentice's last name");
            RuleFor(m => m.StandardCode).NotEmpty().WithMessage("A standard should be selected");
            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999).WithMessage("The UKPRN should contain exactly 8 numbers");
            RuleFor(m => m.Username).NotEmpty();

            RuleFor(m => m.CertificateData.ContactName).NotEmpty().WithMessage("Enter a contact name");
            RuleFor(m => m.CertificateData.ContactOrganisation).NotEmpty().WithMessage("Enter an organisation");
            RuleFor(m => m.CertificateData.ContactAddLine1).NotEmpty().WithMessage("Enter an address");
            RuleFor(m => m.CertificateData.ContactAddLine4).NotEmpty().WithMessage("Enter a city or town");
            RuleFor(m => m.CertificateData.ContactPostCode).NotEmpty().WithMessage("Enter a postcode");
            RuleFor(m => m.CertificateData.ContactPostCode).Matches("^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$").WithMessage("Enter a valid UK postcode");

            RuleFor(m => m.CertificateData.OverallGrade).NotEmpty().WithMessage("Select the grade the apprentice achieved");

            RuleFor(m => m.CertificateData.AchievementDate)
                .Custom((achievementDate, context) =>
                {
                    if (!achievementDate.HasValue)
                    {
                        context.AddFailure(new ValidationFailure("AchievementDate", "Enter the achievement day"));
                    }
                    else if (achievementDate.Value < new DateTime(2017, 1, 1))
                    {
                        context.AddFailure(new ValidationFailure("AchievementDate", "An achievement date cannot be before 01 01 2017"));
                    }
                    else if (achievementDate.Value > DateTime.UtcNow)
                    {
                        context.AddFailure(new ValidationFailure("AchievementDate", "An achievement date cannot be in the future"));
                    }
                });

            RuleFor(m => m)
                .Custom(async (m, context) =>
                {
                    var requestedIlr = await ilrRepository.Get(m.Uln, m.StandardCode);
                    var sumbittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);

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
                        var providedStandards = await assessmentOrgsApiClient.FindAllStandardsByOrganisationIdAsync(sumbittingEpao.EndPointAssessorOrganisationId);

                        if (!providedStandards.Where(s => s.StandardCode == m.StandardCode.ToString()).Any())
                        {
                            context.AddFailure(new ValidationFailure("StandardCode", "EPAO does not provide this Standard"));
                        }
                    }
                });
        }
    }
}
