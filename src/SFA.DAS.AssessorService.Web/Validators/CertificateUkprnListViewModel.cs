using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateUkprnListViewModelValidator : AbstractValidator<CertificateUkprnListViewModel>
    {
        public CertificateUkprnListViewModelValidator(IStringLocalizer<CertificateUkprnListViewModel> localizer)
        {
            RuleFor(vm => vm.SelectedUkprn).NotEmpty().WithMessage(localizer["NameCannotBeEmpty"]);
        }
    }
}
