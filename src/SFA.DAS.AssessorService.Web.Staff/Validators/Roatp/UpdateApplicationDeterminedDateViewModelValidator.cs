using FluentValidation;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Web.Staff.Services.Roatp;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public class UpdateApplicationDeterminedDateViewModelValidator : AbstractValidator<UpdateApplicationDeterminedDateViewModel>
    {


        private readonly IApplicationDeterminedDateValidationService _applicationDeterminedDateValidationService;
        public UpdateApplicationDeterminedDateViewModelValidator(IApplicationDeterminedDateValidationService applicationDeterminedDateValidationService)
        {
            _applicationDeterminedDateValidationService = applicationDeterminedDateValidationService;

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

        private ValidationResponse IsaValidDeterminedDate(UpdateApplicationDeterminedDateViewModel viewModel)
        {
        return _applicationDeterminedDateValidationService.ValidateApplicationDeterminedDate(viewModel.Day,
                viewModel.Month, viewModel.Year);
        }
    }
}