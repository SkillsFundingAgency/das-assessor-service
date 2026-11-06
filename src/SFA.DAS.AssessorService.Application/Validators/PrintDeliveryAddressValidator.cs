using FluentValidation;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Validators
{
    public sealed record PrintDeliveryAddress(
        CertificateSendTo SendTo,
        string RecipientName,
        string OrganisationName,
        string AddressLine1,
        string Postcode);

    public sealed class PrintDeliveryAddressValidator
        : AbstractValidator<PrintDeliveryAddress>
    {
        public PrintDeliveryAddressValidator()
        {
            RuleFor(address => address.SendTo)
                .Must(sendTo => sendTo is
                    CertificateSendTo.Apprentice or CertificateSendTo.Employer)
                .WithMessage("Select who will receive the certificate");

            RuleFor(address => address.RecipientName)
                .NotEmpty()
                .WithMessage("Enter the recipient's name");

            RuleFor(address => address.OrganisationName)
                .NotEmpty()
                .WithMessage("Enter the organisation name")
                .When(address => address.SendTo == CertificateSendTo.Employer);

            RuleFor(address => address.AddressLine1)
                .NotEmpty()
                .WithMessage("Enter address line 1");

            RuleFor(address => address.Postcode)
                .NotEmpty()
                .WithMessage("Enter the postcode");
        }
    }
}