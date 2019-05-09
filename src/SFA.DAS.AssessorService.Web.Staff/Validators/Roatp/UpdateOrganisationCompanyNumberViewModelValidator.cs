using System.Linq;
using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public class UpdateOrganisationCompanyNumberViewModelValidator : AbstractValidator<UpdateOrganisationCompanyNumberViewModel>
    {
        private IRoatpOrganisationValidator _validator;

        public UpdateOrganisationCompanyNumberViewModelValidator(IRoatpOrganisationValidator validator)
        {
            _validator = validator;
    
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationErrors = _validator.IsValidCompanyNumber(vm.CompanyNumber);
                
                if (!validationErrors.Any()) return;
                foreach (var error in validationErrors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
