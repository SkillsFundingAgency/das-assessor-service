using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class CertificateGradeViewModelValidator : AbstractValidator<CertificateGradeViewModel>
    {
        public CertificateGradeViewModelValidator()
        {
            RuleFor(vm => vm.SelectedGrade).NotEmpty().WithMessage("Select the grade the apprentice achieved");
        }
    }
}