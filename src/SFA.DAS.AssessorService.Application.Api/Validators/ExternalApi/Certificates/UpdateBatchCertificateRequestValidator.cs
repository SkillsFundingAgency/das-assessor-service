using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Certificates
{
    public class UpdateBatchCertificateRequestValidator : AbstractValidator<UpdateBatchCertificateRequest>
    {
        public UpdateBatchCertificateRequestValidator(
            IStringLocalizer<BatchCertificateRequestValidator> localiser, 
            IOrganisationQueryRepository organisationQueryRepository, 
            IIlrRepository ilrRepository, 
            ICertificateRepository certificateRepository, 
            IStandardService standardService)
        {
            Include(new BatchCertificateRequestValidator(localiser, organisationQueryRepository, ilrRepository, standardService));

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
                        var certData = JsonConvert.DeserializeObject<CertificateData>(existingCertificate.CertificateData);

                        if (!EpaOutcome.Pass.Equals(certData.EpaDetails?.LatestEpaOutcome, StringComparison.InvariantCultureIgnoreCase))
                        {
                            context.AddFailure(new ValidationFailure("CertificateReference", $"Latest EPA Outcome has not passed"));
                        }
                    }
                    else
                    {
                        context.AddFailure(new ValidationFailure("CertificateReference", $"Certificate does not exist in {CertificateStatus.Draft} status"));
                    }
                });
            });
        }
    }
}
