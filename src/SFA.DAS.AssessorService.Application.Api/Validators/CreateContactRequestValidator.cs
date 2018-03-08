using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class CreateContactRequestValidator : AbstractValidator<CreateContactRequest>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
 
        public CreateContactRequestValidator(IStringLocalizer<CreateContactRequestValidator> localiser,
            IContactQueryRepository contactQueryRepository
        )
        {            
            _contactQueryRepository = contactQueryRepository;

            // ReSharper disable once LocalNameCapturedOnly
            CreateContactRequest createContactRequest;
            RuleFor(contact => contact.Email).NotEmpty().WithMessage(
                localiser[ResourceMessageName.DisplayNameMustBeDefined].Value);
            RuleFor(contact => contact.DisplayName).NotEmpty().WithMessage(
                localiser[ResourceMessageName.EMailMustBeDefined].Value);
            RuleFor(contact => contact.EndPointAssessorOrganisationId).NotEmpty().WithMessage(
                localiser[ResourceMessageName.EndPointAssessorOrganisationIdMustBeDefined].Value);
            RuleFor(contact => contact.Username).NotEmpty().WithMessage(
                localiser[ResourceMessageName.UserNameMustBeDefined].Value);
            RuleFor(contact => contact).Must(NotAlreadyExist).WithMessage(localiser[ResourceMessageName.AlreadyExists].Value);
        }

        private bool NotAlreadyExist(CreateContactRequest contact)
        {
            var result = _contactQueryRepository.CheckContactExists(contact.Username).Result;
            return !result;
        }
    }
}