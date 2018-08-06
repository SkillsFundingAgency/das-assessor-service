using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class CertificateRecipientViewModelValidator : AbstractValidator<CertificateRecipientViewModel>
    {
        public CertificateRecipientViewModelValidator()
        {
            RuleFor(vm => vm.Name).NotEmpty().WithMessage("Name must be defined");           
        }
    }
}