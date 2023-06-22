using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateNameViewModelValidator : AbstractValidator<CertificateNamesViewModel>
    {
        public CertificateNameViewModelValidator()
        {
            RuleFor(vm => vm.InputGivenNames).NotEmpty().WithMessage("Enter the apprentice's first name").WithName("InputGivenNames").DependentRules(() =>
            {
                RuleFor(vm => vm.InputGivenNames.ToLower()).NotEmpty().Equal(vm => vm.GivenNames.ToLower()).WithMessage("Enter the apprentice's first name without changing the original spelling").WithName("InputGivenNames");
            });

            RuleFor(vm => vm.InputFamilyName).NotEmpty().WithMessage("Enter the apprentice's last name").WithName("InputFamilyName").DependentRules(() =>
            {
                RuleFor(vm => vm.InputFamilyName.ToLower()).NotEmpty().Equal(vm => vm.FamilyName.ToLower()).WithMessage("Enter the apprentice's last name without changing the original spelling").WithName("InputFamilyName");
            });
        }
    }
}
