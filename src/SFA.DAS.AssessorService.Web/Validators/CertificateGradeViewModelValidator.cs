using FluentValidation;
using SFA.DAS.AssessorService.Web.Controllers;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateGradeViewModelValidator : AbstractValidator<CertificateGradeViewModel>
    {
        public CertificateGradeViewModelValidator()
        {
            RuleFor(vm => vm.SelectedGrade).NotEmpty().WithMessage("A grade should be selected.");
        }
    }
}