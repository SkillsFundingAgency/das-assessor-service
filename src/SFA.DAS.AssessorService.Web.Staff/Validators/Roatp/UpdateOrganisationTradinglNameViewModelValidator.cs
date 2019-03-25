namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    using System.Linq;
    using FluentValidation;
    using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

    public class UpdateOrganisationTradingNameViewModelValidator : AbstractValidator<UpdateOrganisationTradingNameViewModel>
    {
        private IRoatpOrganisationValidator _validator;

        public UpdateOrganisationTradingNameViewModelValidator(IRoatpOrganisationValidator validator)
        {
            _validator = validator;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationErrors = _validator.IsValidLegalName(vm.TradingName);
                if (!validationErrors.Any()) return;
                foreach (var error in validationErrors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
