using FluentValidation;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Certificates
{
    public class GetBatchCertificateLogsValidator : AbstractValidator<GetBatchCertificateLogsRequest>
    {
        public GetBatchCertificateLogsValidator()
        {
            RuleFor(m => m.CertificateReference).NotNull().WithMessage("Provide certificate id");
        }
    }
}
