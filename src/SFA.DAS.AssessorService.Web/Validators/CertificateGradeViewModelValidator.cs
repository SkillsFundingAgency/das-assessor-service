using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateGradeViewModelValidator : AbstractValidator<CertificateGradeViewModel>
    {
        public CertificateGradeViewModelValidator(IStringLocalizer<CertificateGradeViewModelValidator> localizer)
        {
            RuleFor(vm => vm.SelectedGrade).NotEmpty().WithMessage(localizer["GradeShouldBeSelected"]);
        }
    }
}