using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class BatchCertificateRequestValidator : AbstractValidator<BatchCertificateRequest>
    {
        public BatchCertificateRequestValidator(IStringLocalizer<BatchCertificateRequestValidator> localiser, ICertificateRepository certificateRepository)
        {
            RuleFor(m => m.Uln).LessThanOrEqualTo(9999999999);
            RuleFor(m => m.FamilyName).NotEmpty();
            RuleFor(m => m.StandardCode).NotEmpty();
            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999);
            RuleFor(m => m.Username).NotEmpty();

            RuleFor(m => m.CertificateData.ContactName).NotEmpty();
            RuleFor(m => m.CertificateData.ContactOrganisation).NotEmpty();
            RuleFor(m => m.CertificateData.ContactAddLine1).NotEmpty();
            RuleFor(m => m.CertificateData.ContactPostCode).NotEmpty();
            RuleFor(m => m.CertificateData.ContactPostCode).Matches("^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$").WithMessage("Postcode Invalid");

            RuleFor(m => m.CertificateData.AchievementDate).NotNull().GreaterThanOrEqualTo(m => m.CertificateData.LearningStartDate).LessThanOrEqualTo(m => DateTime.UtcNow);
            RuleFor(m => m.CertificateData.OverallGrade).NotEmpty();

            RuleFor(m => m)
                .Custom((m, context) =>
                {
                    var existingCertificate = certificateRepository.GetCertificate(m.Uln, m.StandardCode).Result;

                    if (existingCertificate != null)
                    {
                        context.AddFailure(new ValidationFailure("CertificateData", $"Certificate already exists: {existingCertificate.CertificateReference}"));
                    }
                });

            // TODO: Add validator for : 'Supplied UkPrn is allowed to access the given endPointAssessorOrganisationId'
            // The following isn't correct. Was dicussed in tech demo to do this later
            //RuleFor(m => m)
            //    .Custom((m, context) =>
            //    {
            //        var sumbittingEpao = organisationQueryRepository.GetByUkPrn(m.UkPrn).Result;
            //        var requestedIlr = ilrRepository.Get(m.Uln, m.StdCode).Result;

            //        if (sumbittingEpao?.EndPointAssessorOrganisationId != requestedIlr?.EpaOrgId)
            //        {
            //            context.AddFailure(new ValidationFailure("UkPrn", "Denined access to the specified Uln"));
            //        }
            //    });
        }
    }
}
