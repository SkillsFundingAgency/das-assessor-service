using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class CertificateGradeViewModelValidator : AbstractValidator<CertificateGradeViewModel>
    {
        public CertificateGradeViewModelValidator(IStringLocalizer<CertificateGradeViewModelValidator> localizer)
        {
            RuleFor(vm => vm.SelectedGrade).NotEmpty().WithMessage("Select the grade the apprentice achieved");
        }
    }
}