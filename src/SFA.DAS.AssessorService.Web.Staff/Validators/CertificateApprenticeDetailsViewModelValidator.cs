using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class CertificateApprenticeDetailsViewModelValidator : AbstractValidator<CertificateApprenticeDetailsViewModel>
    {
        public CertificateApprenticeDetailsViewModelValidator()
        {
            RuleFor(vm => vm.GivenNames).NotEmpty().WithMessage("Given names must be defined");
            RuleFor(vm => vm.FamilyName).NotEmpty().WithMessage("Family Name must be defined");
        }
    }
}
