using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class CertificateStandardCodeListViewModelValidator : AbstractValidator<CertificateStandardCodeListViewModel>
    {
        public CertificateStandardCodeListViewModelValidator()
        {
            RuleFor(vm => vm.SelectedStandardCode).NotEmpty().WithMessage("Select the standard");
        }
    }
}
