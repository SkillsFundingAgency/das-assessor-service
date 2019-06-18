﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Certificates
{
    public class SubmitBatchCertificateRequestValidator : AbstractValidator<SubmitBatchCertificateRequest>
    {
        public SubmitBatchCertificateRequestValidator(IStringLocalizer<SubmitBatchCertificateRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IStandardService standardService)
        {
            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999).WithMessage("The UKPRN should contain exactly 8 numbers");
            RuleFor(m => m.Email).NotEmpty();

            RuleFor(m => m.FamilyName).NotEmpty().WithMessage("Enter the apprentice's last name");
            RuleFor(m => m.StandardCode).GreaterThan(0).WithMessage("A Standard should be selected").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    if (!string.IsNullOrEmpty(m.StandardReference))
                    {
                        var collatedStandard = await standardService.GetStandard(m.StandardReference);
                        if (m.StandardCode != collatedStandard?.StandardId)
                        {
                            context.AddFailure("StandardReference and StandardCode relate to different standards");
                        }
                    }
                });
            });

            RuleFor(m => m.Uln).InclusiveBetween(1000000000, 9999999999).WithMessage("The apprentice's ULN should contain exactly 10 numbers").DependentRules(() =>
            {
                When(m => m.StandardCode > 0 && !string.IsNullOrEmpty(m.FamilyName), () =>
                {
                    RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                    {
                        var requestedIlr = await ilrRepository.Get(m.Uln, m.StandardCode);
                        var sumbittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);

                        if (requestedIlr is null || !string.Equals(requestedIlr.FamilyName, m.FamilyName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            context.AddFailure(new ValidationFailure("Uln", "Cannot find apprentice with the specified Uln, FamilyName & Standard"));
                        }
                        else if (sumbittingEpao is null)
                        {
                            context.AddFailure(new ValidationFailure("UkPrn", "Cannot find EPAO for specified UkPrn"));
                        }
                        else
                        {
                            var providedStandards = await standardService.GetEpaoRegisteredStandards(sumbittingEpao.EndPointAssessorOrganisationId);

                            if (!providedStandards.Any(s => s.StandardCode == m.StandardCode))
                            {
                                context.AddFailure("EPAO is not registered for this Standard");
                            }
                        }
                    });
                });
            });

            RuleFor(m => m.CertificateReference).NotEmpty().WithMessage("Enter the certificate reference").DependentRules(() =>
            {
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
                        context.AddFailure(new ValidationFailure("CertificateReference", $"EPAO is not the creator of this Certificate"));
                    }
                    else if (existingCertificate.Status == CertificateStatus.Submitted || existingCertificate.Status == CertificateStatus.Printed
                                || existingCertificate.Status == CertificateStatus.Reprint)
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate has already been submitted"));
                    }
                    else if (existingCertificate.Status != CertificateStatus.Draft)
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate is not in '{CertificateStatus.Ready}' status"));
                    }
                    else
                    {
                        var certificateData = JsonConvert.DeserializeObject<CertificateData>(existingCertificate.CertificateData);

                        if (certificateData.LearnerGivenNames is null || certificateData.LearnerFamilyName is null || certificateData.ContactName is null ||
                            certificateData.ContactOrganisation is null || certificateData.ContactAddLine1 is null || certificateData.ContactAddLine4 is null ||
                            certificateData.ContactPostCode is null || certificateData.OverallGrade is null || certificateData.AchievementDate is null)
                        {
                            context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate is missing mandatory data"));
                        }
                    }
                });
            });
        }
    }
}
