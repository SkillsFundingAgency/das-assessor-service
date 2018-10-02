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
        public BatchCertificateRequestValidator(IStringLocalizer<BatchCertificateRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IAssessmentOrgsApiClient assessmentOrgsApiClient)
        {
            RuleFor(m => m.Uln).InclusiveBetween(1000000000, 9999999999).WithMessage("The apprentice's ULN should contain exactly 10 numbers");
            RuleFor(m => m.FamilyName).NotEmpty().WithMessage("Enter the apprentice's last name");
            RuleFor(m => m.StandardCode).GreaterThan(0).WithMessage("A standard should be selected");
            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999).WithMessage("The UKPRN should contain exactly 8 numbers");
            RuleFor(m => m.Email).NotEmpty();

            RuleFor(m => m.CertificateData).NotEmpty().WithMessage("Enter Certificate Data").DependentRules(() =>
            {
                RuleFor(m => m.CertificateData.ContactName).NotEmpty().WithMessage("Enter a contact name");
                RuleFor(m => m.CertificateData.ContactOrganisation).NotEmpty().WithMessage("Enter an organisation");
                RuleFor(m => m.CertificateData.ContactAddLine1).NotEmpty().WithMessage("Enter an address");
                RuleFor(m => m.CertificateData.ContactAddLine4).NotEmpty().WithMessage("Enter a city or town");
                RuleFor(m => m.CertificateData.ContactPostCode).NotEmpty().WithMessage("Enter a postcode");
                RuleFor(m => m.CertificateData.ContactPostCode).Matches("^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$").WithMessage("Enter a valid UK postcode");

                RuleFor(m => m.CertificateData.OverallGrade)
                    .Custom((overallGrade, context) =>
                    {
                        var grades = new string[] { "Pass", "Merit", "Distinction", "Pass with excellence", "No grade awarded" };

                        if (string.IsNullOrWhiteSpace(overallGrade))
                        {
                            context.AddFailure(new ValidationFailure("OverallGrade", "Select the grade the apprentice achieved"));
                        }
                        else if (!grades.Any(g => g == overallGrade))
                        {
                            string gradesString = string.Join(", ", grades);
                            context.AddFailure(new ValidationFailure("OverallGrade", $"Invalid grade. Must be one of the following: {gradesString}"));
                        }
                    });

                RuleFor(m => m.CertificateData.AchievementDate)
                    .Custom((achievementDate, context) =>
                    {
                        if (!achievementDate.HasValue)
                        {
                            context.AddFailure(new ValidationFailure("AchievementDate", "Enter the achievement date"));
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

                        if (!providedStandards.Any(s => s.StandardCode == m.StandardCode.ToString()))
                        {
                            context.AddFailure(new ValidationFailure("StandardCode", "EPAO does not provide this Standard"));
                        }
                    }
                });

            RuleFor(m => m)
                .Custom((m, context) =>
                {
                    var courseOptions = certificateRepository.GetOptions(m.StandardCode).GetAwaiter().GetResult();

                    if (!courseOptions.Any() && !string.IsNullOrEmpty(m.CertificateData?.CourseOption))
                    {
                        context.AddFailure(new ValidationFailure("CourseOption", $"Invalid course option for this Standard. Must be empty"));
                    }
                    else if (courseOptions.Any() && !courseOptions.Any(o => o.OptionName == m.CertificateData?.CourseOption))
                    {
                        string courseOptionsString = string.Join(", ", courseOptions.Select(o => o.OptionName));
                        context.AddFailure(new ValidationFailure("CourseOption", $"Invalid course option for this Standard. Must be one of the following: {courseOptionsString}"));
                    }
                });
        }
    }
}
