namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    using FluentValidation;
    using SFA.DAS.AssessorService.Api.Types.Models.Validation;
    using SFA.DAS.AssessorService.Web.Staff.Resources;
    using System.Collections.Generic;
    using ViewModels.Roatp;

    public class AddOrganisationProviderTypeViewModelValidator : AbstractValidator<AddOrganisationProviderTypeViewModel>
    {
        public AddOrganisationProviderTypeViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult = IsValidProviderType(vm);
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }

        private ValidationResponse IsValidProviderType(AddOrganisationProviderTypeViewModel viewModel)
        {
            var validationResult = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };
            
            var isValid = (viewModel.ProviderTypeId >= 1 && viewModel.ProviderTypeId <= 3);

            if (!isValid)
            {
                validationResult.Errors.Add(new ValidationErrorDetail("ProviderTypeId", RoatpOrganisationValidation.InvalidProviderTypeId));
            }

            return validationResult;
        }
    }
}
