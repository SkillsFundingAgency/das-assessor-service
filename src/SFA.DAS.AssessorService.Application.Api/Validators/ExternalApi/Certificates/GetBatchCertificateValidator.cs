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

            RuleFor(m => m.FamilyName).NotEmpty().WithMessage("Provide apprentice family name");
            RuleFor(m => m.StandardCode).GreaterThan(0).WithMessage("Provide a valid Standard");

            RuleFor(m => m.Uln).InclusiveBetween(1000000000, 9999999999).WithMessage("ULN should contain exactly 10 numbers").DependentRules(() =>
            {
                When(m => m.StandardCode > 0 && !string.IsNullOrEmpty(m.FamilyName), () =>
                {
                    RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                    {
                        // NOTE: Currently we're making the Certificate & ILR record both mandatory - this is wrong fixing it!
                        var submittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);
                       
                        if (submittingEpao is null)
                        {
                            context.AddFailure(new ValidationFailure("UkPrn", "Specified UKPRN not found"));
                        }
                        else
                        {
                            var existingCertificateCreatedByCallingEpao = await certificateRepository.GetCertificateByOrgIdLastname(m.Uln, submittingEpao.EndPointAssessorOrganisationId, m.FamilyName);

                            if (existingCertificateCreatedByCallingEpao == null)
                            {
                                var requestedIlr = await ilrRepository.Get(m.Uln, m.StandardCode);
                                var existingCertificate = await certificateRepository.GetCertificate(m.Uln, m.StandardCode);
                                var providedStandards = await standardService.GetEpaoRegisteredStandards(submittingEpao.EndPointAssessorOrganisationId);

                                if (!providedStandards.Any(s => s.StandardCode == m.StandardCode))
                                {
                                    context.AddFailure(new ValidationFailure("StandardCode", "Your organisation is not approved to assess this Standard"));
                                }

                                if (existingCertificate != null)
                                {
                                    var certData = JsonConvert.DeserializeObject<CertificateData>(existingCertificate.CertificateData ?? "{}");

                                    if (!certData.LearnerFamilyName.Equals(m.FamilyName, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        context.AddFailure(new ValidationFailure("FamilyName", $"Cannot find apprentice with the specified Uln, FamilyName & Standard"));
                                    }
                                    else if (!EpaOutcome.Pass.Equals(certData.EpaDetails?.LatestEpaOutcome, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        context.AddFailure(new ValidationFailure("Uln", $"Cannot find certificate with the specified Uln, FamilyName & Standard"));
                                    }
                                }
                                else if (requestedIlr is null || !string.Equals(requestedIlr.FamilyName, m.FamilyName, StringComparison.InvariantCultureIgnoreCase) )
                                {
                                    context.AddFailure(new ValidationFailure("Uln", "Cannot find apprentice with the specified Uln, FamilyName & Standard"));
                                }
                            }
                        }
                    });
                });
            });
        }
    }
}
