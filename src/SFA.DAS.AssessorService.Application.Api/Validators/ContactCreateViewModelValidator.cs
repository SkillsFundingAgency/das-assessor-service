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

            var organisationCreateViewModel = new CreateContactRequest();
            RuleFor(contact => contact.ContactEmail).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.ContactNameMustBeDefined,
                    nameof(organisationCreateViewModel.ContactName)].Value);
            RuleFor(contact => contact.ContactName).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.ContactEMailMustBeDefined,
                    nameof(organisationCreateViewModel.ContactEmail)].Value);
            RuleFor(contact => contact.EndPointAssessorContactId).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.EndPointAssessorContactIdMustBeDefined,
                    nameof(organisationCreateViewModel.EndPointAssessorContactId)].Value);
            RuleFor(contact => contact).Must(NotAlreadyExist).WithMessage(_localizer[ResourceMessageName.AlreadyExists,
                nameof(organisationCreateViewModel)].Value);
        }

        private bool NotAlreadyExist(CreateContactRequest contact)
        {
            var result = _contactQueryRepository.CheckContactExists(contact.ContactName).Result;
            return !result;
        }
    }
}