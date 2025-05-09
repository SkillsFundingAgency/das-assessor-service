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
    public class CreateBatchCertificateRequestValidator : AbstractValidator<CreateBatchCertificateRequest>
    {
        public CreateBatchCertificateRequestValidator(
            IStringLocalizer<BatchCertificateRequestValidator> localiser,
            IOrganisationQueryRepository organisationQueryRepository,
            ILearnerRepository learnerRepository,
            ICertificateRepository certificateRepository,
            IStandardService standardService)
        {
            Include(new BatchCertificateRequestValidator(localiser, organisationQueryRepository, learnerRepository, standardService));

            RuleFor(m => m.CertificateReference).Empty().WithMessage("Certificate reference must be empty").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    var existingCertificate = await certificateRepository.GetCertificate(m.Uln, m.StandardCode);
                    var sumbittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);
                    var learnerDetails = await learnerRepository.Get(m.Uln, m.StandardCode);

                    if (existingCertificate != null && existingCertificate.Status != CertificateStatus.Deleted)
                    {
                        if (sumbittingEpao?.Id != existingCertificate.OrganisationId)
                        {
                            context.AddFailure(new ValidationFailure("CertificateData", $"Your organisation is not the creator of the EPA"));
                        }
                        else if (existingCertificate.Status == CertificateStatus.Draft)
                        {
                            if (!string.IsNullOrEmpty(existingCertificate.CertificateData.OverallGrade))
                            {
                                context.AddFailure(new ValidationFailure("CertificateData", $"Certificate already exists: {existingCertificate.CertificateReference}"));
                            }
                        }
                        else if (
                            existingCertificate.Status == CertificateStatus.Submitted && 
                            !string.IsNullOrWhiteSpace(existingCertificate.CertificateData.OverallGrade) && 
                            existingCertificate.CertificateData.OverallGrade.Equals(CertificateGrade.Fail, StringComparison.OrdinalIgnoreCase))
                        { 
                            // A submitted fail can be re-created
                        }
                        else
                        {
                            context.AddFailure(new ValidationFailure("CertificateData", $"Certificate already exists: {existingCertificate.CertificateReference}"));
                        }
                    }

                    if (learnerDetails != null)
                    {
                        if (learnerDetails.CompletionStatus == (int)CompletionStatus.Withdrawn)
                        {
                            context.AddFailure(new ValidationFailure("LearnerDetails", "Cannot find the apprentice details"));
                        }
                        else if (learnerDetails.CompletionStatus == (int)CompletionStatus.TemporarilyWithdrawn)
                        {
                            context.AddFailure(new ValidationFailure("LearnerDetails", "Cannot find the apprentice details"));
                        }

                        // SV-1253 additional validation to check version and option
                        if(learnerDetails.VersionConfirmed && !string.IsNullOrWhiteSpace(learnerDetails.Version) && !string.IsNullOrWhiteSpace(m.CertificateData.Version))
                        {
                            if(learnerDetails.Version.Trim().ToUpperInvariant() != m.CertificateData.Version.Trim().ToUpperInvariant())
                            {
                                context.AddFailure(new ValidationFailure("LearnerDetails", "Incorrect version for learner"));
                            }
                        }
                        if(!string.IsNullOrWhiteSpace(learnerDetails.CourseOption) && !string.IsNullOrWhiteSpace(m.CertificateData.CourseOption))
                        {
                            if(learnerDetails.CourseOption.Trim().ToUpperInvariant() != m.CertificateData.CourseOption.Trim().ToUpperInvariant())
                            {
                                context.AddFailure(new ValidationFailure("LearnerDetails", "Incorrect course option for learner"));
                            }
                        }
                    }
                    else
                    {
                        context.AddFailure(new ValidationFailure("LearnerDetails", $"Learner details for ULN: {m.Uln} not found"));
                    }

                });
            });
        }
    }
}