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

            // ReSharper disable once LocalNameCapturedOnly
            CreateContactRequest createContactRequest;
            RuleFor(organisation => organisation.Email).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.DisplayNameMustBeDefined,
                    nameof(createContactRequest.DisplayName)].Value);
            RuleFor(organisation => organisation.DisplayName).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.EMailMustBeDefined,
                    nameof(createContactRequest.Email)].Value);
            RuleFor(organisation => organisation.Username).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.UserNameMustBeDefined,
                    nameof(createContactRequest.Username)].Value);
        }
    }
}