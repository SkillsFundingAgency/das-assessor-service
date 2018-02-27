namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    using AssessorService.Api.Types.Models;
    using Consts;
    using FluentValidation;
    using Microsoft.Extensions.Localization;

    public class ContactUpdateViewModelValidator : AbstractValidator<UpdateContactRequest>
    {
        private readonly IStringLocalizer<ContactUpdateViewModelValidator> _localizer;

        public ContactUpdateViewModelValidator(IStringLocalizer<ContactUpdateViewModelValidator> localizer
        )
        {
            _localizer = localizer;

            var organisationCreateViewModel = new CreateContactRequest();
            RuleFor(organisation => organisation.ContactEmail).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.ContactNameMustBeDefined,
                    nameof(organisationCreateViewModel.ContactName)].Value);
            RuleFor(organisation => organisation.ContactName).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.ContactEMailMustBeDefined,
                    nameof(organisationCreateViewModel.ContactEmail)].Value);
        }
    }
}