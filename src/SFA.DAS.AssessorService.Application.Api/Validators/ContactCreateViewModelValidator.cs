using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class ContactCreateViewModelValidator : AbstractValidator<CreateContactRequest>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
 
        public ContactCreateViewModelValidator(IStringLocalizer<ContactCreateViewModelValidator> localiser,
            IContactQueryRepository contactQueryRepository
        )
        {            
            _contactQueryRepository = contactQueryRepository;

            // ReSharper disable once LocalNameCapturedOnly
            CreateContactRequest createContactRequest;
            RuleFor(contact => contact.Email).NotEmpty().WithMessage(
                localiser[ResourceMessageName.DisplayNameMustBeDefined,
                    nameof(createContactRequest.DisplayName)].Value);
            RuleFor(contact => contact.DisplayName).NotEmpty().WithMessage(
                localiser[ResourceMessageName.EMailMustBeDefined,
                    nameof(createContactRequest.Email)].Value);
            RuleFor(contact => contact.EndPointAssessorOrganisationId).NotEmpty().WithMessage(
                localiser[ResourceMessageName.EndPointAssessorOrganisationIdMustBeDefined,
                    nameof(createContactRequest.EndPointAssessorOrganisationId)].Value);
            RuleFor(contact => contact.Username).NotEmpty().WithMessage(
                localiser[ResourceMessageName.UserNameMustBeDefined,
                    nameof(createContactRequest.Username)].Value);
            RuleFor(contact => contact).Must(NotAlreadyExist).WithMessage(localiser[ResourceMessageName.AlreadyExists,
                nameof(createContactRequest)].Value);
        }

        private bool NotAlreadyExist(CreateContactRequest contact)
        {
            var result = _contactQueryRepository.CheckContactExists(contact.Username).Result;
            return !result;
        }
    }
}