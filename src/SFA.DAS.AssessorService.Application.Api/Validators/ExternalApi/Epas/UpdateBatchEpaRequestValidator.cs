using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using System;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Epas
{
    public class UpdateBatchEpaRequestValidator : AbstractValidator<BatchEpaRequest>
    {
        public UpdateBatchEpaRequestValidator(IStringLocalizer<BatchEpaRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IStandardService standardService)
        {
            Include(new BatchEpaRequestValidator(localiser, organisationQueryRepository, ilrRepository, standardService));

            RuleFor(m => m.EpaDetails.EpaReference).NotEmpty().WithMessage("Provide EPA reference").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    var existingCertificate = await certificateRepository.GetCertificate(m.Uln, m.StandardCode);
                    var sumbittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);

                    if (existingCertificate is null || !string.Equals(existingCertificate.CertificateReference, m.EpaDetails.EpaReference, StringComparison.InvariantCultureIgnoreCase)
                        || existingCertificate.Status == CertificateStatus.Deleted)
                    {
                        context.AddFailure(new ValidationFailure("EpaReference", $"EPA not found"));
                    }
                    else if (sumbittingEpao?.Id != existingCertificate.OrganisationId)
                    {
                        context.AddFailure(new ValidationFailure("EpaReference", $"Your organisation is not the creator of this EPA"));
                    }
                    else if (existingCertificate.Status != CertificateStatus.Draft)
                    {
                        context.AddFailure(new ValidationFailure("EpaReference", $"Certificate has been submitted for this EPA"));
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////
                    // TODO: Need to redo this taking into account if a certificate has been requested then stop
                    // 
                    // TODO: Add various unit tests to cover this and any other scenario
                    ////////////////////////////////////////////////////////////////////////////////////////////
                });
            });
        }
    }
}
