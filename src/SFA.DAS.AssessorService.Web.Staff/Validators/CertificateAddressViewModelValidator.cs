using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateAddressViewModelValidator : AbstractValidator<CertificateAddressViewModel>
    {
        public CertificateAddressViewModelValidator()
        {           
            RuleFor(vm => vm.Employer).NotEmpty().WithMessage("Enter an organisation");
            RuleFor(vm => vm.Postcode).NotEmpty().WithMessage("Enter a postcode");
            RuleFor(vm => vm.Postcode).Matches("^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$").WithMessage("Enter a valid UK postcode");
            RuleFor(vm => vm.AddressLine1).NotEmpty().WithMessage("Enter an address");
            RuleFor(vm => vm.City).NotEmpty().WithMessage("Enter a city or town");
        }
    }
}