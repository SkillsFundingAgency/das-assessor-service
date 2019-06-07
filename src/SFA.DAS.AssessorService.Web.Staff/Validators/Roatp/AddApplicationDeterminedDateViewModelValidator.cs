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
    public class AddApplicationDeterminedDateViewModelValidator : AbstractValidator<AddApplicationDeterminedDateViewModel>
    {
        public AddApplicationDeterminedDateViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult = IsaValidDeterminedDate(vm);
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }

        private ValidationResponse IsaValidDeterminedDate(AddApplicationDeterminedDateViewModel viewModel)
        {
            var validationResult = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            //var isValid = (viewModel.ProviderTypeId >= 1 && viewModel.ProviderTypeId <= 3);

            //if (!isValid)
            //{
            //    validationResult.Errors.Add(new ValidationErrorDetail("ProviderTypeId", RoatpOrganisationValidation.InvalidProviderTypeId));
            //}
           // validationResult.Errors.Add(new ValidationErrorDetail("Day", "Day Message"));
            
        
            return validationResult;
        }
    }
}
