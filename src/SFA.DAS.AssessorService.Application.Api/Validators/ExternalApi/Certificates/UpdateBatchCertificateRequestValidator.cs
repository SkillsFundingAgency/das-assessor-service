﻿using System;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Certificates
{
    public class UpdateBatchCertificateRequestValidator : AbstractValidator<UpdateBatchCertificateRequest>
    {
        public UpdateBatchCertificateRequestValidator(
            IStringLocalizer<BatchCertificateRequestValidator> localiser, 
            IOrganisationQueryRepository organisationQueryRepository,
            ILearnerRepository learnerRepository, 
            ICertificateRepository certificateRepository, 
            IStandardService standardService)
        {
            Include(new BatchCertificateRequestValidator(localiser, organisationQueryRepository, learnerRepository, standardService));

            RuleFor(m => m.CertificateReference).NotEmpty().WithMessage("Provide the certificate reference").DependentRules(() =>
            {
                // TODO: ON-2130 Consider in the future how to merge both create & update versions as the Cert will always exist due to EPA 
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    var existingCertificate = await certificateRepository.GetCertificate(m.Uln, m.StandardCode);
                    var sumbittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);

                    if (existingCertificate is null || !string.Equals(existingCertificate.CertificateReference, m.CertificateReference, StringComparison.InvariantCultureIgnoreCase)
                        || existingCertificate.Status == CertificateStatus.Deleted)
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate not found"));
                    }
                    else if (sumbittingEpao?.Id != existingCertificate.OrganisationId)
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Your organisation is not the creator of this Certificate"));
                    }
                    else if (existingCertificate.Status == CertificateStatus.Draft)
                    {
                        if (string.IsNullOrEmpty(existingCertificate.CertificateData.OverallGrade))
                        {
                            context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate not found"));
                        }
                    }
                    else
                    {
                        if (existingCertificate.CertificateData.OverallGrade == CertificateGrade.Fail)
                        {
                            context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate not found"));
                        }
                        else
                        {
                            context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate does not exist in {CertificateStatus.Draft} status"));
                        }
                    }

                    // SV-1253 additional validation to check version and option
                    var learnerDetails = await learnerRepository.Get(m.Uln, m.StandardCode);
                    if(null != learnerDetails)
                    {
                        if (learnerDetails.VersionConfirmed && !string.IsNullOrWhiteSpace(learnerDetails.Version) && !string.IsNullOrWhiteSpace(m.CertificateData.Version))
                        {
                            if (learnerDetails.Version != m.CertificateData.Version)
                            {
                                context.AddFailure(new ValidationFailure("LearnerDetails", "Incorrect version for learner"));
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(learnerDetails.CourseOption) && !string.IsNullOrWhiteSpace(m.CertificateData.CourseOption))
                        {
                            if (learnerDetails.CourseOption != m.CertificateData.CourseOption)
                            {
                                context.AddFailure(new ValidationFailure("LearnerDetails", "Incorrect course option for learner"));
                            }
                        }
                    }
                });
            });
        }
    }
}
