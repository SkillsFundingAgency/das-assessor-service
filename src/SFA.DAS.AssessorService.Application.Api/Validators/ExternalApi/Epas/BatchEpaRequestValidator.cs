using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Epas
{
    public class BatchEpaRequestValidator : AbstractValidator<BatchEpaRequest>
    {
        public BatchEpaRequestValidator(IStringLocalizer<BatchEpaRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, IStandardService standardService)
        {
            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999).WithMessage("The UKPRN should contain exactly 8 numbers");

            RuleFor(m => m.FamilyName).NotEmpty().WithMessage("Provide apprentice family name");
            RuleFor(m => m.StandardCode).GreaterThan(0).WithMessage("Provide a valid Standard").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    if (!string.IsNullOrEmpty(m.StandardReference))
                    {
                        var standard = await standardService.GetStandardVersionById(m.StandardReference);
                        if (m.StandardCode != standard?.LarsCode)
                        {
                            context.AddFailure("StandardReference and StandardCode must be for the same Standard");
                        }
                    }
                });
            });

            When(m => !string.IsNullOrWhiteSpace(m.CourseOption) && string.IsNullOrWhiteSpace(m.Version), () =>
            {
                RuleFor(m => m.CourseOption).Custom((m, context) =>
                {
                    context.AddFailure(new ValidationFailure("CourseOption", "Version must be set, when submitting a CourseOption value"));
                });
            });

            When(m => !string.IsNullOrWhiteSpace(m.Version), () =>
            {
                RuleFor(m => m).Custom((m, context) =>
                {
                    // StandardUId is set if version is supplied and a valid version is found.
                    if (string.IsNullOrWhiteSpace(m.StandardUId))
                    {
                        context.AddFailure(new ValidationFailure("Standard", "Invalid version for Standard"));
                    }
                });

                When(m => !string.IsNullOrWhiteSpace(m.CourseOption), () =>
                {
                    RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                    {
                        if (!string.IsNullOrWhiteSpace(m.StandardUId))
                        {
                            var standardOptions = await standardService.GetStandardOptionsByStandardId(m.StandardUId);

                            if(standardOptions != null && !standardOptions.HasOptions())
                            {
                                context.AddFailure(new ValidationFailure("CourseOption", "No course option available for this Standard and version. Must be empty"));
                            }
                            else if (standardOptions != null && standardOptions.HasOptions() && standardOptions.CourseOption.All(a => a.IndexOf(m.CourseOption, StringComparison.OrdinalIgnoreCase) == -1))
                            {
                                var validOptions = string.Join(",", standardOptions.CourseOption);
                                context.AddFailure(new ValidationFailure("CourseOption",
                                    $@"Invalid course option for this Standard and version. 
                                     Must be one of the following: '{validOptions}' where '{validOptions}' depends on the standard code, 
                                     and can be obtained with GET /api/v1/standard/options/{m.StandardReference}/{m.Version}"));
                            }
                        }
                    });
                });
            });

            RuleFor(m => m.Uln).InclusiveBetween(1000000000, 9999999999).WithMessage("ULN should contain exactly 10 numbers").DependentRules(() =>
            {
                When(m => m.StandardCode > 0 && !string.IsNullOrEmpty(m.FamilyName), () =>
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
                        else if (requestedIlr.CompletionStatus == (int)CompletionStatus.Withdrawn)
                        {
                            context.AddFailure(new ValidationFailure("LearnerDetails", "Cannot find the apprentice details"));
                        }
                        else if (requestedIlr.CompletionStatus == (int)CompletionStatus.TemporarilyWithdrawn)
                        {
                            context.AddFailure(new ValidationFailure("LearnerDetails", "Cannot find the apprentice details"));
                        }
                        else
                        {
                            var providedStandardVersions = await standardService.GetEPAORegisteredStandardVersions(sumbittingEpao.EndPointAssessorOrganisationId, m.StandardCode);

                            if (!providedStandardVersions.Any())
                            {
                                context.AddFailure(new ValidationFailure("StandardCode", "Your organisation is not approved to assess this Standard"));
                            }
                            else if (!string.IsNullOrWhiteSpace(m.Version) && providedStandardVersions.Any(v => v.Version.Equals(m.Version, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                context.AddFailure(new ValidationFailure("Version", "Your organisation is not approved to assess this Standard Version"));
                            }
                        }
                    });
                });
            });

            RuleFor(m => m.EpaDetails).NotEmpty().WithMessage("Provide EPA Details").DependentRules(() =>
            {
                var earliestDate = new DateTime(2017, 1, 1);
                var latestDate = DateTime.UtcNow;
                var outcomes = new string[] { EpaOutcome.Pass, EpaOutcome.Fail, EpaOutcome.Withdrawn };

                RuleFor(m => m.EpaDetails.Epas).NotEmpty().WithMessage("Provide EPA Details")
                    .ForEach(epaRule =>
                    {
                        epaRule.Must(epa => epa.EpaDate >= earliestDate).WithMessage("EPA Date cannot be before 01 01 2017")
                               .Must(epa => epa.EpaDate <= latestDate).WithMessage("EPA Date cannot be in the future")
                               .Must(epa => outcomes.Contains(epa.EpaOutcome, StringComparer.InvariantCultureIgnoreCase)).WithMessage($"Invalid outcome: must be Pass, Fail or Withdrawn");
                    });
            });
        }
    }
}
