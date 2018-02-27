namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    using FluentValidation;
    using Microsoft.Extensions.Localization;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class ContactCreateViewModelValidator : AbstractValidator<CreateContactRequest>
    {
        private readonly IStringLocalizer<ContactCreateViewModelValidator> _localizer;
        private readonly IContactQueryRepository _contactQueryRepository;

        public ContactCreateViewModelValidator(IStringLocalizer<ContactCreateViewModelValidator> localizer,
             IContactQueryRepository contactQueryRepository
            ) : base()
        {
            _localizer = localizer;
            _contactQueryRepository = contactQueryRepository;

            var organisationCreateViewModel = new CreateContactRequest();
            RuleFor(contact => contact.ContactEmail).NotEmpty().WithMessage(_localizer[ResourceMessageName.ContactNameMustBeDefined, nameof(organisationCreateViewModel.ContactName)].Value);
            RuleFor(contact => contact.ContactName).NotEmpty().WithMessage(_localizer[ResourceMessageName.ContactEMailMustBeDefined, nameof(organisationCreateViewModel.ContactEmail)].Value);
            RuleFor(contact => contact.EndPointAssessorContactId).NotEmpty().WithMessage(_localizer[ResourceMessageName.EndPointAssessorContactIdMustBeDefined, nameof(organisationCreateViewModel.EndPointAssessorContactId)].Value);
            RuleFor(contact => contact).Must(NotAlreadyExists).WithMessage(_localizer[ResourceMessageName.AlreadyExists, nameof(organisationCreateViewModel)].Value);
        }

        private bool NotAlreadyExists(CreateContactRequest contact)
        {
            var result = _contactQueryRepository.CheckContactExists(contact.ContactName, contact.ContactEmail).Result;
            return !result;
        }
    }
}
