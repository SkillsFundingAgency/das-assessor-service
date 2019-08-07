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
            RuleFor(m => m.Email).NotEmpty().WithMessage("Provide your Email address");

            RuleFor(m => m.FamilyName).NotEmpty().WithMessage("Provide apprentice family name");
            RuleFor(m => m.StandardCode).GreaterThan(0).WithMessage("Provide a Standard").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    if (!string.IsNullOrEmpty(m.StandardReference))
                    {
                        var collatedStandard = await standardService.GetStandard(m.StandardReference);
                        if (m.StandardCode != collatedStandard?.StandardId)
                        {
                            context.AddFailure("StandardReference and StandardCode must be for the same Standard");
                        }
                    }
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
