using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class CertificateFirstNameViewModelValidator : AbstractValidator<CertificateFirstNameViewModel>
    {
        public CertificateFirstNameViewModelValidator()
        {
            RuleFor(vm => vm.FirstName).NotEmpty().WithMessage("Enter the apprentice’s first name");
        }
    }
}
