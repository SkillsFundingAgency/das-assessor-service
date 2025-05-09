﻿using System;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Epas
{
    public class UpdateBatchEpaRequestValidator : AbstractValidator<UpdateBatchEpaRequest>
    {
        public UpdateBatchEpaRequestValidator(IStringLocalizer<BatchEpaRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, ILearnerRepository learnerRepository, ICertificateRepository certificateRepository, IStandardService standardService)
        {
            Include(new BatchEpaRequestValidator(localiser, organisationQueryRepository, learnerRepository, standardService));

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
                                if(!string.IsNullOrWhiteSpace(existingCertificate.CertificateData.OverallGrade))
                                {
                                    context.AddFailure(new ValidationFailure("EpaReference", $"Certificate already exists, cannot update EPA record"));
                                }
                                if (string.IsNullOrWhiteSpace(existingCertificate.CertificateData.EpaDetails?.LatestEpaOutcome))
                                {
                                    context.AddFailure(new ValidationFailure("EpaReference", $"EPA not found"));
                                }
                                break;
                            default:
                                // Submitted but a fail, can be updated.
                                if (existingCertificate.CertificateData.OverallGrade != EpaOutcome.Fail)
                                {
                                    context.AddFailure(new ValidationFailure("EpaReference", $"Certificate already exists, cannot update EPA record"));
                                }
                                break;
                        }
                    }
                });
            });
        }
    }
}
