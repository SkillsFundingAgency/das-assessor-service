using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public class UpdateOrganisationCharityNumberViewModelValidator : AbstractValidator<UpdateOrganisationCharityNumberViewModel>
    {
        private IRoatpOrganisationValidator _validator;

        public UpdateOrganisationCharityNumberViewModelValidator(IRoatpOrganisationValidator validator)
        {
            _validator = validator;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationErrors = _validator.IsValidCharityNumber(vm.CharityNumber);

                if (!validationErrors.Any()) return;
                foreach (var error in validationErrors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }


            });
        }
    }
}