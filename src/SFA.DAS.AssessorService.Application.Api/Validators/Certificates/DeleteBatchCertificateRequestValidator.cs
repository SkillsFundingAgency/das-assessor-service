using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Api.Validators.Certificates
{
    public class DeleteBatchCertificateRequestValidator : AbstractValidator<DeleteBatchCertificateRequest>
    {
        public DeleteBatchCertificateRequestValidator(IStringLocalizer<DeleteBatchCertificateRequestValidator> localiser, ICertificateRepository certificateRepository)
        {
            RuleFor(m => m.Uln).LessThanOrEqualTo(9999999999);
            RuleFor(m => m.FamilyName).NotEmpty();
            RuleFor(m => m.StandardCode).NotEmpty();
            RuleFor(m => m.UkPrn).InclusiveBetween(10000000, 99999999);
            RuleFor(m => m.Username).NotEmpty();

            RuleFor(m => m)
                .Custom((m, context) =>
                {
                    var existingCertificate = certificateRepository.GetCertificate(m.Uln, m.StandardCode).Result;

                    if (existingCertificate == null)
                    {
                        context.AddFailure(new ValidationFailure("Certificate", $"Certificate not found"));
                    }
                    else if (existingCertificate.Status == CertificateStatus.Submitted)
                    {
                        context.AddFailure(new ValidationFailure("Certificate", $"Certificate cannot be Deleted when in '{CertificateStatus.Submitted}' status"));
                    }
                    else if (existingCertificate.Status == CertificateStatus.Printed)
                    {
                        context.AddFailure(new ValidationFailure("Certificate", $"Certificate cannot be Deleted when in '{CertificateStatus.Printed}' status"));
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
