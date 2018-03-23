using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateAddressViewModelValidator : AbstractValidator<CertificateAddressViewModel>
    {
        public CertificateAddressViewModelValidator(IStringLocalizer<CertificateAddressViewModelValidator> localizer)
        {
            RuleFor(vm => vm.Name).NotEmpty().WithMessage(localizer["NameCannotBeEmpty"]);
            RuleFor(vm => vm.Employer).NotEmpty().WithMessage(localizer["EmployerCannotBeEmpty"]);
            RuleFor(vm => vm.Postcode).NotEmpty().WithMessage(localizer["PostcodeCannotBeEmpty"]);
            RuleFor(vm => vm.AddressLine1).NotEmpty().WithMessage(localizer["AddressLine1CannotBeEmpty"]);
            RuleFor(vm => vm.City).NotEmpty().WithMessage(localizer["CityCannotBeEmpty"]);
        }
    }
}