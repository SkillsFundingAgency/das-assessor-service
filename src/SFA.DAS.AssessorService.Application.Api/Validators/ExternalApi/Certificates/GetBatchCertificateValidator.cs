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
    public class GetBatchCertificateRequestValidator : AbstractValidator<GetBatchCertificateRequest>
    {
        public GetBatchCertificateRequestValidator(IStringLocalizer<GetBatchCertificateRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IStandardService standardService)
        {
            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999).WithMessage("The UKPRN should contain exactly 8 numbers");
            RuleFor(m => m.Email).NotEmpty();

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
                    RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                    {
                        // NOTE: Currently we're making the Certificate & ILR record both mandatory
                        var requestedIlr = await ilrRepository.Get(m.Uln, m.StandardCode);
                        var sumbittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);

                        if (requestedIlr is null || !string.Equals(requestedIlr.FamilyName, m.FamilyName, StringComparison.InvariantCultureIgnoreCase))
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

                    RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                    {
                        var existingCertificate = await certificateRepository.GetCertificate(m.Uln, m.StandardCode);

                        if (existingCertificate is null)
                        {
                            // TODO: FUTURE WORK - ON-2130 Do Alan's Certificate Search THEN the ILR Search (which may be the validation down below)
                        }
                        else
                        {
                            var certData = JsonConvert.DeserializeObject<CertificateData>(existingCertificate.CertificateData ?? "{}");

                            if (!certData.LearnerFamilyName.Equals(m.FamilyName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                context.AddFailure(new ValidationFailure("FamilyName", $"Invalid family name"));
                            }
                            else if (!EpaOutcome.Pass.Equals(certData.EpaDetails?.LatestEpaOutcome, StringComparison.InvariantCultureIgnoreCase))
                            {
                                context.AddFailure(new ValidationFailure("Uln", $"Latest EPA Outcome has not passed"));
                            }
                        }
                    });
                });
            });
        }
    }
}
