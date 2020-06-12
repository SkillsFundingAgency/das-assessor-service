using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Certificates
{
    public class BatchCertificateRequestValidator : AbstractValidator<BatchCertificateRequest>
    {
        public BatchCertificateRequestValidator(
            IStringLocalizer<BatchCertificateRequestValidator> localiser, 
            IOrganisationQueryRepository organisationQueryRepository, 
            IIlrRepository ilrRepository,  
            IStandardService standardService)
        {
            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999).WithMessage("The UKPRN should contain exactly 8 numbers");

            RuleFor(m => m.FamilyName).NotEmpty().WithMessage("Provide apprentice family name");
            RuleFor(m => m.StandardCode).GreaterThan(0).WithMessage("Provide a valid Standard").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    bool validateCourseOption = true;

                    var collatedStandard = await standardService.GetStandard(m.StandardCode);
                    if (!string.IsNullOrEmpty(m.StandardReference))
                    {
                        if (m.StandardReference != collatedStandard?.ReferenceNumber)
                        {
                            validateCourseOption = false;
                            context.AddFailure("StandardReference and StandardCode must be for the same Standard");
                        }
                    }

                    // NOTE: This is not a nice way to do this BUT we cannot use another DependantRules()
                    if (validateCourseOption)
                    {
                        if (!collatedStandard.Options.Any() && !string.IsNullOrEmpty(m.CertificateData?.CourseOption))
                        {
                            context.AddFailure(new ValidationFailure("CourseOption", $"No course option available for this Standard. Must be empty"));
                        }
                        else if (collatedStandard.Options.Any() && !collatedStandard.Options.Any(o => o.Equals(m.CertificateData?.CourseOption, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            string courseOptionsString = string.Join(", ", collatedStandard.Options);
                            context.AddFailure(new ValidationFailure("CourseOption", $"Invalid course option for this Standard. Must be one of the following: {courseOptionsString}"));
                        }
                    }
                });
            });

            RuleFor(m => m.Uln).InclusiveBetween(1000000000, 9999999999).WithMessage("ULN should contain exactly 10 numbers").DependentRules(() =>
            {
                When(m => m.StandardCode > 0 && !string.IsNullOrEmpty(m.FamilyName) , () =>
               {
                   RuleFor(m => m).CustomAsync(async (m, context, canellation) =>
                   {
                       var requestedIlr = await ilrRepository.Get(m.Uln, m.StandardCode);
                       var sumbittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);

                       if (requestedIlr is null || !string.Equals(requestedIlr.FamilyName, m.FamilyName, StringComparison.InvariantCultureIgnoreCase))
                       {
                           context.AddFailure(new ValidationFailure("Uln", "ULN, FamilyName and Standard not found."));
                       }
                       else if (sumbittingEpao is null)
                       {
                           context.AddFailure(new ValidationFailure("UkPrn", "Specified UKPRN not found"));
                       }
                       else
                       {
                           var providedStandards = await standardService.GetEpaoRegisteredStandards(sumbittingEpao.EndPointAssessorOrganisationId);

                           if (!providedStandards.Any(s => s.StandardCode == m.StandardCode))
                           {
                               context.AddFailure(new ValidationFailure("StandardCode", "Your organisation is not approved to assess this Standard"));
                           }
                       }
                   });
               });
            });

            RuleFor(m => m.CertificateData).NotEmpty().WithMessage("Provide Certificate Data").DependentRules(() =>
            {
                RuleFor(m => m.CertificateData.ContactName).NotEmpty().WithMessage("Provide a contact name");
                RuleFor(m => m.CertificateData.ContactOrganisation).NotEmpty().WithMessage("Provide an organisation");
                RuleFor(m => m.CertificateData.ContactAddLine1).NotEmpty().WithMessage("Provide an address");
                RuleFor(m => m.CertificateData.ContactAddLine4).NotEmpty().WithMessage("Provide a city or town");
                RuleFor(m => m.CertificateData.ContactPostCode).NotEmpty().WithMessage("Provide a postcode").DependentRules(() =>
                { 
                    RuleFor(m => m.CertificateData.ContactPostCode).Matches("^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$").WithMessage("Provide a valid UK postcode");
                });

                RuleFor(m => m.CertificateData.OverallGrade)
                    .Custom((overallGrade, context) =>
                    {
                        var grades = new string[] { CertificateGrade.Pass, CertificateGrade.Credit, CertificateGrade.Merit, CertificateGrade.Distinction, CertificateGrade.PassWithExcellence, CertificateGrade.Outstanding, CertificateGrade.NoGradeAwarded };

                        if (string.IsNullOrWhiteSpace(overallGrade))
                        {
                            context.AddFailure(new ValidationFailure("OverallGrade", "Select the grade the apprentice achieved"));
                        }
                        else if (overallGrade.Equals(CertificateGrade.Fail, StringComparison.InvariantCultureIgnoreCase))
                        {
                            context.AddFailure(new ValidationFailure("OverallGrade", $"Cannot record a {CertificateGrade.Fail} grade via API"));
                        }
                        else if (!grades.Contains(overallGrade, StringComparer.InvariantCultureIgnoreCase))
                        {
                            string gradesString = string.Join(", ", grades);
                            context.AddFailure(new ValidationFailure("OverallGrade", $"You must enter a valid grade. Must be one of the following: {gradesString}"));
                        }
                    });

                RuleFor(m => m.CertificateData.AchievementDate)
                    .Custom((achievementDate, context) =>
                    {
                        if (!achievementDate.HasValue)
                        {
                            context.AddFailure(new ValidationFailure("AchievementDate", "Provide the achievement date"));
                        }
                        else if (achievementDate.Value < new DateTime(2017, 1, 1))
                        {
                            context.AddFailure(new ValidationFailure("AchievementDate", "Achievement date cannot be before 01 01 2017"));
                        }
                        else if (achievementDate.Value > DateTime.UtcNow)
                        {
                            context.AddFailure(new ValidationFailure("AchievementDate", "Achievement date cannot be in the future"));
                        }
                    });
            });
        }
    }
}
