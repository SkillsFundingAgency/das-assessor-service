using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateAddressViewModelValidator : AbstractValidator<CertificateAddressViewModel>
    {
        public CertificateAddressViewModelValidator(IStringLocalizer<CertificateAddressViewModelValidator> localizer)
        {
            When(vm => !vm.SelectPreviousAddress, () =>
            {
                RuleFor(vm => vm.Postcode).NotEmpty()
                    .WithMessage(localizer["PostcodeCannotBeEmpty"]);
                RuleFor(vm => vm.Postcode)
                    .Matches(
                        "^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$")
                    .WithMessage(localizer["PostcodeValid"]);
                RuleFor(vm => vm.AddressLine1).NotEmpty()
                    .WithMessage(localizer["AddressLine1CannotBeEmpty"]);
                RuleFor(vm => vm.City).NotEmpty()
                    .WithMessage(localizer["CityCannotBeEmpty"]);
            });
        }
    }
}