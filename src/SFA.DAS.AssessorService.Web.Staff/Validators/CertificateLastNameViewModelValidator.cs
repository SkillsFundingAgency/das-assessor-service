using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class CertificateLastNameViewModelValidator : AbstractValidator<CertificateLastNameViewModel>
    {
        public CertificateLastNameViewModelValidator()
        {
            RuleFor(vm => vm.LastName).NotEmpty().WithMessage("Enter the apprentice’s last name");
        }
    }
}
