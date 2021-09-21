using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Certificates
{
    public class SubmitBatchCertificateRequestValidator : AbstractValidator<SubmitBatchCertificateRequest>
    {
        public SubmitBatchCertificateRequestValidator(IStringLocalizer<SubmitBatchCertificateRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, ILearnerRepository learnerRepository, ICertificateRepository certificateRepository, IStandardService standardService)
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
                                context.AddFailure("Your organisation is not approved to assess this Standard");
                            }
                        }
                    });
                });
            });

            RuleFor(m => m.CertificateReference).NotEmpty().WithMessage("Provide the certificate reference").DependentRules(() =>
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
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Your organisation is not the creator of this Certificate"));
                    }
                    else if (existingCertificate.Status == CertificateStatus.Submitted || CertificateStatus.HasPrintProcessStatus(existingCertificate.Status))
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate has already been submitted"));
                    }
                    else if (existingCertificate.Status != CertificateStatus.Draft)
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate is not in {CertificateStatus.Ready} status"));
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
