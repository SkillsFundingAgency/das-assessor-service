using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class CertificateApprenticeDetailsViewModelValidator : AbstractValidator<CertificateApprenticeDetailsViewModel>
    {
        public CertificateApprenticeDetailsViewModelValidator()
        {
            RuleFor(vm => vm.GivenNames).NotEmpty().WithMessage("Enter a first name");
            RuleFor(vm => vm.FamilyName).NotEmpty().WithMessage("Enter a last name");
        }
    }
}
