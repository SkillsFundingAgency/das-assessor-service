using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class UpdateContactRequestValidator : AbstractValidator<UpdateContactRequest>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
 
        public UpdateContactRequestValidator(IStringLocalizer<UpdateContactRequestValidator> localiser,
            IContactQueryRepository contactQueryRepository
        )
        {            
            _contactQueryRepository = contactQueryRepository;

            // ReSharper disable once LocalNameCapturedOnly
            UpdateContactRequest updateContactRequest;
            RuleFor(contact => contact.Email).NotEmpty().WithMessage(
                localiser[ResourceMessageName.DisplayNameMustBeDefined,
                    nameof(updateContactRequest.DisplayName)].Value);
            RuleFor(contact => contact.DisplayName).NotEmpty().WithMessage(
                localiser[ResourceMessageName.EMailMustBeDefined,
                    nameof(updateContactRequest.Email)].Value);
            RuleFor(contact => contact.Username).NotEmpty().WithMessage(
                localiser[ResourceMessageName.UserNameMustBeDefined,
                    nameof(updateContactRequest.Username)].Value);
            RuleFor(contact => contact).Must(AlreadyExist).WithMessage(localiser[ResourceMessageName.AlreadyExists,
                nameof(updateContactRequest)].Value);
        }

        private bool AlreadyExist(UpdateContactRequest contact)
        {
            var result = _contactQueryRepository.CheckContactExists(contact.Username).Result;
            return result;
        }
    }
}