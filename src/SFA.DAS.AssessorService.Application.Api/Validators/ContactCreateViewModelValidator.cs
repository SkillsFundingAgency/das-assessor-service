namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    using AssessorService.Api.Types.Models;
    using Consts;
    using FluentValidation;
    using Interfaces;
    using Microsoft.Extensions.Localization;

    public class ContactCreateViewModelValidator : AbstractValidator<CreateContactRequest>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IStringLocalizer<ContactCreateViewModelValidator> _localizer;

        public ContactCreateViewModelValidator(IStringLocalizer<ContactCreateViewModelValidator> localizer,
            IContactQueryRepository contactQueryRepository
        )
        {
            _localizer = localizer;
            _contactQueryRepository = contactQueryRepository;

            // ReSharper disable once LocalNameCapturedOnly
            CreateContactRequest createContactRequest;
            RuleFor(contact => contact.ContactEmail).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.ContactNameMustBeDefined,
                    nameof(createContactRequest.ContactName)].Value);
            RuleFor(contact => contact.ContactName).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.ContactEMailMustBeDefined,
                    nameof(createContactRequest.ContactEmail)].Value);
            RuleFor(contact => contact.EndPointAssessorContactId).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.EndPointAssessorContactIdMustBeDefined,
                    nameof(createContactRequest.EndPointAssessorContactId)].Value);
            RuleFor(contact => contact).Must(NotAlreadyExist).WithMessage(_localizer[ResourceMessageName.AlreadyExists,
                nameof(createContactRequest)].Value);
        }

        private bool NotAlreadyExist(CreateContactRequest contact)
        {
            var result = _contactQueryRepository.CheckContactExists(contact.ContactName).Result;
            return !result;
        }
    }
}