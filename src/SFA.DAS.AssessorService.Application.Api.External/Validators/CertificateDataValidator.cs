using System;
using FluentValidation;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Api.External.Validators
{
    public class CertificateDataValidator : AbstractValidator<CertificateData>
    {
        public CertificateDataValidator()
        {
            RuleFor(m => m.LearnerGivenNames).NotEmpty();
            RuleFor(m => m.LearnerFamilyName).NotEmpty();

            RuleFor(m => m.StandardName).NotEmpty();
            RuleFor(m => m.StandardLevel).GreaterThanOrEqualTo(0);
            RuleFor(m => m.StandardPublicationDate).LessThanOrEqualTo(DateTime.Now);

            RuleFor(m => m.ContactName).NotEmpty();
            RuleFor(m => m.ContactOrganisation).NotEmpty();
            RuleFor(m => m.ContactAddLine1).NotEmpty();

            RuleFor(m => m.ContactPostCode).NotEmpty();
            RuleFor(m => m.ContactPostCode).Matches("^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$").WithMessage("Postcode Invalid");

            RuleFor(m => m.LearningStartDate).LessThanOrEqualTo(DateTime.Now);
            RuleFor(m => m.AchievementDate).NotNull().GreaterThanOrEqualTo(m => m.LearningStartDate);

            RuleFor(m => m.CourseOption).NotEmpty();
            RuleFor(m => m.OverallGrade).NotEmpty();
            RuleFor(m => m.Department).NotEmpty();
            RuleFor(m => m.FullName).NotEmpty();
            RuleFor(m => m.ProviderName).NotEmpty();
        }
    }
}