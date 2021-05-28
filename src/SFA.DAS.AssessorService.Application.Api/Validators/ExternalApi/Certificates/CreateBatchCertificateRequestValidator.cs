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
    public class CreateBatchCertificateRequestValidator : AbstractValidator<CreateBatchCertificateRequest>
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
                    var learnerDetails = await ilrRepository.Get(m.Uln, m.StandardCode);

                    if (existingCertificate != null && existingCertificate.Status != CertificateStatus.Deleted)
                    {
                        var certData = JsonConvert.DeserializeObject<CertificateData>(existingCertificate.CertificateData);

                        if (sumbittingEpao?.Id != existingCertificate.OrganisationId)
                        {
                            context.AddFailure(new ValidationFailure("CertificateData", $"Your organisation is not the creator of the EPA"));
                        }
                        else if (existingCertificate.Status == CertificateStatus.Draft)
                        {
                            if (!string.IsNullOrEmpty(certData.OverallGrade))
                            {
                                context.AddFailure(new ValidationFailure("CertificateData", $"Certificate already exists: {existingCertificate.CertificateReference}"));
                            }
                        }
                        else if (existingCertificate.Status == CertificateStatus.Submitted && !string.IsNullOrWhiteSpace(certData.OverallGrade) && certData.OverallGrade.Equals(CertificateGrade.Fail, StringComparison.OrdinalIgnoreCase))
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