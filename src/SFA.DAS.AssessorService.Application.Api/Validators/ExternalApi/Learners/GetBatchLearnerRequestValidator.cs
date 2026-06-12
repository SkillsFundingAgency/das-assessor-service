using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Learners
{
    public class GetBatchLearnerRequestValidator : AbstractValidator<GetBatchLearnerRequest>
    {
        public GetBatchLearnerRequestValidator(IStringLocalizer<GetBatchLearnerRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, ILearnerRepository learnerRepository, ICertificateRepository certificateRepository, IStandardService standardService)
        {
            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999).WithMessage("The UKPRN should contain exactly 8 numbers");

            RuleFor(m => m.FamilyName).NotEmpty().WithMessage("Provide apprentice family name");
            RuleFor(m => m.Standard).NotEmpty().WithMessage("Provide a Standard").DependentRules(() =>
            {
                RuleFor(m => m.Standard).CustomAsync(async (standard, context, cancellation) =>
                {
                    var standardVersion = await standardService.GetStandardVersionById(standard);

                    if (standardVersion is null)
                    {
                        context.AddFailure(new ValidationFailure("Standard", "Standard not found"));
                    }
                });
            });

            RuleFor(m => m.Uln).InclusiveBetween(1000000000, 9999999999).WithMessage("ULN should contain exactly 10 numbers").DependentRules(() =>
            {
                When(m => !string.IsNullOrEmpty(m.Standard) && !string.IsNullOrEmpty(m.FamilyName), () =>
                {
                    RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                    {
                        var standard = await standardService.GetStandardVersionById(m.Standard);

                        if (standard != null)
                        {
                            var requestedLearner = await learnerRepository.Get(m.Uln, standard.LarsCode);
                            var sumbittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);

                            if (requestedLearner is null || !string.Equals(requestedLearner.FamilyName, m.FamilyName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                context.AddFailure(new ValidationFailure("Uln", "ULN, FamilyName and Standard not found"));
                            }
                            else if (sumbittingEpao is null)
                            {
                                context.AddFailure(new ValidationFailure("UkPrn", "Specified UKPRN not found"));
                            }
                            else
                            {
                                var providedStandards = await standardService.GetEpaoRegisteredStandards(sumbittingEpao.EndPointAssessorOrganisationId);

                                if (!providedStandards.Any(s => s.StandardCode == standard.LarsCode))
                                {
                                    context.AddFailure(new ValidationFailure("StandardCode", "Your organisation is not approved to assess this Standard"));
                                }
                            }
                        }
                    });
                });
            });
        }
    }
}
