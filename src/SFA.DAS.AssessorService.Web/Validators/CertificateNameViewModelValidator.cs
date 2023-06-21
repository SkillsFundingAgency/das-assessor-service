using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateNameViewModelValidator : AbstractValidator<CertificateNamesViewModel>
    {
        public CertificateNameViewModelValidator()
        {
            RuleFor(vm => vm.InputGivenNames).NotEmpty().WithMessage("First names must not be empty").WithName("InputGivenNames").DependentRules(() =>
            {
                RuleFor(vm => vm.InputGivenNames.ToLower()).NotEmpty().Equal(vm => vm.GivenNames.ToLower()).WithMessage("You can only change the casing of the first names").WithName("InputGivenNames");
            });

            RuleFor(vm => vm.InputFamilyName).NotEmpty().WithMessage("Last name must not be empty").WithName("InputFamilyName").DependentRules(() =>
            {
                RuleFor(vm => vm.InputFamilyName.ToLower()).NotEmpty().Equal(vm => vm.FamilyName.ToLower()).WithMessage("You can only change the casing of the last name").WithName("InputFamilyName");
            });
        }
    }
}
