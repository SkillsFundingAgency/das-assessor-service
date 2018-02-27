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
            RuleFor(organisation => organisation.ContactEmail).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.ContactNameMustBeDefined,
                    nameof(createContactRequest.ContactName)].Value);
            RuleFor(organisation => organisation.ContactName).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.ContactEMailMustBeDefined,
                    nameof(createContactRequest.ContactEmail)].Value);
        }
    }
}