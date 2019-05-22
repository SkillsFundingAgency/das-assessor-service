using System.Linq;
using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public class UpdateOrganisationCharityNumberViewModelValidator : AbstractValidator<UpdateOrganisationCharityNumberViewModel>
    {
        private readonly IRoatpOrganisationValidator _validator;
        private readonly IUpdateOrganisationCharityNumberValidator _duplicateValidator;
        public UpdateOrganisationCharityNumberViewModelValidator(IRoatpOrganisationValidator validator, IUpdateOrganisationCharityNumberValidator duplicateValidator)
        {
            _validator = validator;
            _duplicateValidator = duplicateValidator;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationErrors = _validator.IsValidCharityNumber(vm.CharityNumber);
                if (!validationErrors.Any())
                {
                    validationErrors = _duplicateValidator.IsDuplicateCharityNumber(vm);
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