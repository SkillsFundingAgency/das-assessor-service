using FluentValidation;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

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