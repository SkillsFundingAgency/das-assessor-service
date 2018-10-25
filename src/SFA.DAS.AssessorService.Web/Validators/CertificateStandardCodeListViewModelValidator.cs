using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateStandardCodeListViewModelValidator : AbstractValidator<CertificateStandardCodeListViewModel>
    {
        public CertificateStandardCodeListViewModelValidator(IStringLocalizer<CertificateStandardCodeListViewModelValidator> localizer)
        {
            RuleFor(vm => vm.SelectedStandardCode).NotEmpty().WithMessage(localizer["NameCannotBeEmpty"]);
        }
    }
}
