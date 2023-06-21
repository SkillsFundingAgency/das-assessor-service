using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateNameViewModelValidator : AbstractValidator<CertificateNamesViewModel>
    {
        public CertificateNameViewModelValidator()
        {
            RuleFor(vm => vm.GivenNames).NotEmpty().WithMessage("First names must not be empty").WithName("GivenNames").DependentRules(() =>
            {
                RuleFor(vm => vm.GivenNames.ToLower()).NotEmpty().Equal(vm => vm.PreviousGivenNames.ToLower()).WithMessage("You can only change the casing of the first names").WithName("GivenNames");
            });

            RuleFor(vm => vm.FamilyName).NotEmpty().WithMessage("Last name must not be empty").WithName("FamilyName").DependentRules(() =>
            {
                RuleFor(vm => vm.FamilyName.ToLower()).NotEmpty().Equal(vm => vm.PreviousFamilyName.ToLower()).WithMessage("You can only change the casing of the last name").WithName("FamilyName");
            });
        }
    }
}
