using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateCheckViewModelValidator : AbstractValidator<CertificateCheckViewModel>
    {
        public CertificateCheckViewModelValidator(IStringLocalizer<CertificateCheckViewModelValidator> localizer)
        {
            When(vm => vm.SelectedGrade != CertificateGrade.Fail, () =>
            {
                RuleFor(vm => vm.Postcode).NotEmpty()
                    .WithMessage(localizer["PostcodeCannotBeEmpty"]);
                RuleFor(vm => vm.City).NotEmpty()
                    .WithMessage(localizer["CityCannotBeEmpty"]);
                RuleFor(vm => vm.AddressLine1).NotEmpty()
                    .WithMessage(localizer["AddressLine1CannotBeEmpty"]);
                RuleFor(vm => vm.Name).NotEmpty()
                .WithMessage(localizer["NameCannotBeEmpty"]);
            });

            When(vm => vm.StandardHasOptions, () =>
            {
                RuleFor(vm => vm.Option).NotNull()
                    .WithMessage(localizer["SelectAnOption"]);
            });
        }
    }
}