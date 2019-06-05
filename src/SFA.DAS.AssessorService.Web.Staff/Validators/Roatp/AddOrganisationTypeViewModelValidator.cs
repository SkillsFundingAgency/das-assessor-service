using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Web.Staff.Resources;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public class AddOrganisationTypeViewModelValidator : AbstractValidator<AddOrganisationTypeViewModel>
    {
        public AddOrganisationTypeViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult = IsValidOrganisationType(vm);
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }

        private ValidationResponse IsValidOrganisationType(AddOrganisationTypeViewModel viewModel)
        {
            var validationResult = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            var isValid = (viewModel.OrganisationTypeId>=0);

            if (!isValid)
            {
                validationResult.Errors.Add(new ValidationErrorDetail("OrganisationTypeId", RoatpOrganisationValidation.InvalidOrganisationTypeId));
            }

            return validationResult;
        }
    }
}
