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
            ILearnerRepository learnerRepository,
            IStandardService standardService)
        {
            bool invalidVersionOrStandardMismatch = false;

            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999).WithMessage("The UKPRN should contain exactly 8 numbers");

            RuleFor(m => m.FamilyName).NotEmpty().WithMessage("Provide apprentice family name");
            RuleFor(m => m.StandardCode).GreaterThan(0).WithMessage("Provide a valid Standard").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    var standard = await standardService.GetStandardVersionById(m.StandardCode.ToString(), m.CertificateData.Version);
                    if (!string.IsNullOrEmpty(m.StandardReference))
                    {
                        if (m.StandardReference != standard?.IfateReferenceNumber)
                        {
                            invalidVersionOrStandardMismatch = true;
                            context.AddFailure("StandardReference and StandardCode must be for the same Standard");
                        }
                    }

                    if (!invalidVersionOrStandardMismatch && standard != null)
                    {
                        var standardOptions = await standardService.GetStandardOptionsByStandardId(standard.StandardUId);
                        var noOptions = standardOptions == null || !standardOptions.HasOptions();
                        var hasOptions = standardOptions != null && standardOptions.HasOptions();
                        if (noOptions && !string.IsNullOrEmpty(m.CertificateData?.CourseOption))
                        {
                            context.AddFailure(new ValidationFailure("CourseOption", $"No course option available for this Standard and version. Must be empty"));
                        }
                        else if (hasOptions && !standardOptions.CourseOption.Any(o => o.Equals(m.CertificateData?.CourseOption, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            string courseOptionsString = string.Join(", ", standardOptions.CourseOption);
                            context.AddFailure(new ValidationFailure("CourseOption", $"Invalid course option for this Standard and version. Must be one of the following: {courseOptionsString} where {courseOptionsString} depends on the standard code, and can be obtained with GET /api/v1/standard/options/{standard.LarsCode}/{standard.Version}"));
                        }
                    }
                });
            });

            When(m => !string.IsNullOrWhiteSpace(m.CertificateData.Version), () =>
            {
                RuleFor(m => m).Custom((m, context) =>
                {
                    // If Version specified but StandardUId not populated, must be invalid version
                    // Otherwise we assume the auto-select process succeeded.
                    if (string.IsNullOrWhiteSpace(m.StandardUId) && !invalidVersionOrStandardMismatch)
                    {
                        invalidVersionOrStandardMismatch = true;
                        context.AddFailure(new ValidationFailure("Standard", "Invalid version for Standard"));
                    }
                });
            });

            RuleFor(m => m.Uln).InclusiveBetween(1000000000, 9999999999).WithMessage("ULN should contain exactly 10 numbers").DependentRules(() =>
            {
                When(m => m.StandardCode > 0 && !string.IsNullOrEmpty(m.FamilyName), () =>
              {
                  RuleFor(m => m).CustomAsync(async (m, context, canellation) =>
                  {
                      var requestedLearner = await learnerRepository.Get(m.Uln, m.StandardCode);
                      var submittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);

                      if (requestedLearner is null || !string.Equals(requestedLearner.FamilyName, m.FamilyName, StringComparison.InvariantCultureIgnoreCase))
                      {
                          context.AddFailure(new ValidationFailure("Uln", "ULN, FamilyName and Standard not found."));
                      }
                      else if (submittingEpao is null)
                      {
                          context.AddFailure(new ValidationFailure("UkPrn", "Specified UKPRN not found"));
                      }
                      else
                      {
                          var providedStandardVersions = await standardService.GetEPAORegisteredStandardVersions(submittingEpao.EndPointAssessorOrganisationId, m.StandardCode);

                          if (!providedStandardVersions.Any())
                          {
                              context.AddFailure(new ValidationFailure("StandardCode", "Your organisation is not approved to assess this Standard"));
                          }
                          else if (!(invalidVersionOrStandardMismatch || providedStandardVersions.Any(v => v.Version.Equals(m.CertificateData.Version, StringComparison.InvariantCultureIgnoreCase))))
                          {
                              context.AddFailure(new ValidationFailure("Version", $"Your organisation is not approved to assess this Standard Version: {m.CertificateData.Version}"));
                          }
                      }
                  });
              });
            });

            RuleFor(m => m.CertificateData).NotEmpty().WithMessage("Provide Certificate Data").DependentRules(() =>
            {
                RuleFor(m => m.CertificateData.ContactName).NotEmpty().WithMessage("Provide a contact name");
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
                            context.AddFailure(new ValidationFailure("OverallGrade", "Select the grade achieved"));
                        }
                        else if (overallGrade.Equals(CertificateGrade.Fail, StringComparison.InvariantCultureIgnoreCase))
                        {
                            context.AddFailure(new ValidationFailure("OverallGrade", $"Cannot record a {CertificateGrade.Fail} grade via API"));
                        }
                        else if (!grades.Contains(overallGrade, StringComparer.InvariantCultureIgnoreCase))
                        {
                            string gradesString = string.Join(", ", grades);
                            context.AddFailure(new ValidationFailure("OverallGrade", $"You must enter a valid grade. Must be one of the following: {gradesString}, where {gradesString} can be obtained with /api/v1/certificate/grades"));
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
