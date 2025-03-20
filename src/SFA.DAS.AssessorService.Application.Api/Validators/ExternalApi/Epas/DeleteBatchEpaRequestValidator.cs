using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Epas
{
    public class DeleteBatchEpaRequestValidator : AbstractValidator<DeleteBatchEpaRequest>
    {
        public DeleteBatchEpaRequestValidator(IStringLocalizer<DeleteBatchEpaRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, ILearnerRepository learnerRepository, ICertificateRepository certificateRepository, IStandardService standardService)
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

            RuleFor(m => m.Uln).InclusiveBetween(1000000000, 9999999999).WithMessage("ULN should contain exactly 10 numbers").DependentRules(() =>
            {
                When(m => m.StandardCode > 0 && !string.IsNullOrEmpty(m.FamilyName), () =>
                {
                    RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                    {
                        var requestedLearner = await learnerRepository.Get(m.Uln, m.StandardCode);
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

                            if (!providedStandards.Any(s => s.StandardCode == m.StandardCode))
                            {
                                context.AddFailure(new ValidationFailure("StandardCode", "Your organisation is not approved to assess this Standard"));
                            }
                        }
                    });
                });
            });

            RuleFor(m => m.EpaReference).NotEmpty().WithMessage("Provide the EPA reference").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    var existingCertificate = await certificateRepository.GetCertificate(m.Uln, m.StandardCode);
                    var sumbittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);

                    if (existingCertificate is null || !string.Equals(existingCertificate.CertificateReference, m.EpaReference, StringComparison.InvariantCultureIgnoreCase))
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
                            case CertificateStatus.Draft:
                                break;
                            default:
                                if (!string.IsNullOrEmpty(existingCertificate.CertificateData.OverallGrade) && 
                                    existingCertificate.CertificateData.OverallGrade != CertificateGrade.Fail)
                                {
                                    context.AddFailure(new ValidationFailure("EpaReference", $"Certificate already exists, cannot delete EPA record"));
                                }
                                break;
                        }
                    }
                });
            });
        }
    }
}
