using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Certificates
{
    public class SubmitBatchCertificateRequestValidator : AbstractValidator<BatchCertificateRequest>
    {
        public SubmitBatchCertificateRequestValidator(IStringLocalizer<BatchCertificateRequestValidator> localiser, ICertificateRepository certificateRepository)
        {
            // NOTE: Submit is effectively an Update at this point in time
            Include(new UpdateBatchCertificateRequestValidator(localiser, certificateRepository));
        }
    }
}
