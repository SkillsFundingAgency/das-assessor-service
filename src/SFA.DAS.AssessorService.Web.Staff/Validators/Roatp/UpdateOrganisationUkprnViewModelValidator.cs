using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public class UpdateOrganisationUkprnViewModelValidator : AbstractValidator<UpdateOrganisationUkprnViewModel>
    {
        private IRoatpOrganisationValidator _validator;
        private IUpdateOrganisationUkprnValidator _duplicateValidator;

        public UpdateOrganisationUkprnViewModelValidator(IRoatpOrganisationValidator validator, IUpdateOrganisationUkprnValidator duplicateValidator)
        {
            _validator = validator;
            _duplicateValidator = duplicateValidator;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationErrors = _validator.IsValidUKPRN(vm.Ukprn.ToString());
                if (!validationErrors.Any())
                {
                    validationErrors = _duplicateValidator.IsDuplicateUkprn(vm);
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
