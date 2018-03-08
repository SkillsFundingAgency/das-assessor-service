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
                localiser[ResourceMessageName.EMailMustBeDefined].Value);

            RuleFor(contact => contact.DisplayName).NotEmpty().WithMessage(
                localiser[ResourceMessageName.DisplayNameMustBeDefined].Value);

            RuleFor(contact => contact.Username).NotEmpty().WithMessage(
                localiser[ResourceMessageName.UserNameMustBeDefined].Value);

            RuleFor(contact => contact.Username).Must(AlreadyExist).WithMessage(localiser[ResourceMessageName.DoesNotExist].Value);
        }

        private bool AlreadyExist(string userName)
        {
            var result = _contactQueryRepository.CheckContactExists(userName).Result;
            return result;
        }
    }
}