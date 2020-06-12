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
    public class CreateBatchCertificateRequestValidator : AbstractValidator<BatchCertificateRequest>
    {
        public CreateBatchCertificateRequestValidator(
            IStringLocalizer<BatchCertificateRequestValidator> localiser,
            IOrganisationQueryRepository organisationQueryRepository, 
            IIlrRepository ilrRepository, 
            ICertificateRepository certificateRepository, 
            IStandardService standardService)
        {
            Include(new BatchCertificateRequestValidator(localiser, organisationQueryRepository, ilrRepository, standardService));

            RuleFor(m => m.CertificateReference).Empty().WithMessage("Certificate reference must be empty").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    var existingCertificate = await certificateRepository.GetCertificate(m.Uln, m.StandardCode);
                    var sumbittingEpao = await organisationQueryRepository.GetByUkPrn(m.UkPrn);

                    if (existingCertificate != null && existingCertificate.Status != CertificateStatus.Deleted)
                    {
                        if (sumbittingEpao?.Id != existingCertificate.OrganisationId)
                        {
                            context.AddFailure(new ValidationFailure("CertificateData", $"Your organisation is not the creator of the EPA"));
                        }
                        else if (existingCertificate.Status == CertificateStatus.Draft)
                        {
                            var certData = JsonConvert.DeserializeObject<CertificateData>(existingCertificate.CertificateData);

                            if (!string.IsNullOrEmpty(certData.OverallGrade) && certData.AchievementDate.HasValue && !string.IsNullOrEmpty(certData.ContactPostCode))
                            {
                                context.AddFailure(new ValidationFailure("CertificateData", $"Certificate already exists: {existingCertificate.CertificateReference}"));
                            }
                            else if (!EpaOutcome.Pass.Equals(certData.EpaDetails?.LatestEpaOutcome, StringComparison.InvariantCultureIgnoreCase))
                            {
                                context.AddFailure(new ValidationFailure("CertificateData", $"Latest EPA Outcome has not passed"));
                            }
                        }
                        else
                        {
                            context.AddFailure(new ValidationFailure("CertificateData", $"Certificate already exists: {existingCertificate.CertificateReference}"));
                        }
                    }
                });
            });
        }
    }
}