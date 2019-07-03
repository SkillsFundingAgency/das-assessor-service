using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Epas
{
    public class CreateBatchEpaRequestValidator : AbstractValidator<BatchEpaRequest>
    {
        public CreateBatchEpaRequestValidator(IStringLocalizer<BatchEpaRequestValidator> localiser, IOrganisationQueryRepository organisationQueryRepository, IIlrRepository ilrRepository, ICertificateRepository certificateRepository, IStandardService standardService)
        {
            Include(new BatchEpaRequestValidator(localiser, organisationQueryRepository, ilrRepository, standardService));

            RuleFor(m => m.EpaDetails.EpaReference).Empty().WithMessage("EPA reference must be empty").DependentRules(() =>
            {
                RuleFor(m => m).CustomAsync(async (m, context, cancellation) =>
                {
                    var existingCertificate = await certificateRepository.GetCertificate(m.Uln, m.StandardCode);

                    if (existingCertificate != null && existingCertificate.Status != CertificateStatus.Deleted)
                    {
                        ///////////////////////////////////////////////////////////////////////////////////
                        // TODO: Need to redo this taking into account - if a certificate has been requested then stop
                        // 
                        // TODO: Add various unit tests to cover this and any other scenario
                        ///////////////////////////////////////////////////////////////////////////////////


                        var certData = JsonConvert.DeserializeObject<CertificateData>(existingCertificate.CertificateData);

                        if (certData?.EpaDetails?.Epas != null)
                        {
                            context.AddFailure(new ValidationFailure("EpaDetails", $"EPA already provided for the learner"));
                        }
                    }
                });
            });
        }
    }
}
