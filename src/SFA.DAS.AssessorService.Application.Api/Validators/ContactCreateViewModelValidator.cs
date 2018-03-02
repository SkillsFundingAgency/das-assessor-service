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
            RuleFor(contact => contact.Email).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.DisplayNameMustBeDefined,
                    nameof(createContactRequest.DisplayName)].Value);
            RuleFor(contact => contact.DisplayName).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.EMailMustBeDefined,
                    nameof(createContactRequest.Email)].Value);
            RuleFor(contact => contact.EndPointAssessorOrganisationId).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.EndPointAssessorOrganisationIdMustBeDefined,
                    nameof(createContactRequest.EndPointAssessorOrganisationId)].Value);
            RuleFor(contact => contact.Username).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.UserNameMustBeDefined,
                    nameof(createContactRequest.Username)].Value);
            RuleFor(contact => contact).Must(NotAlreadyExist).WithMessage(_localizer[ResourceMessageName.AlreadyExists,
                nameof(createContactRequest)].Value);
        }

        private bool NotAlreadyExist(CreateContactRequest contact)
        {
            var result = _contactQueryRepository.CheckContactExists(contact.Username).Result;
            return !result;
        }
    }
}