using System.Linq;
using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public class UpdateOrganisationCompanyNumberViewModelValidator : AbstractValidator<UpdateOrganisationCompanyNumberViewModel>
    {
        private IRoatpOrganisationValidator _validator;
        private IUpdateOrganisationCompanyNumberValidator _duplicateValidator;

        public UpdateOrganisationCompanyNumberViewModelValidator(IRoatpOrganisationValidator validator, IUpdateOrganisationCompanyNumberValidator duplicateValidator)
        {
            _validator = validator;
            _duplicateValidator = duplicateValidator;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationErrors = _validator.IsValidCompanyNumber(vm.CompanyNumber);
                if (!validationErrors.Any())
                {
                    validationErrors = _duplicateValidator.IsDuplicateCompanyNumber(vm);
                }
                if (!validationErrors.Any()) return;
                foreach (var error in validationErrors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
