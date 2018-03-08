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

            UpdateContactRequest updateContactRequest;
            // ReSharper disable once LocalNameCapturedOnly
            RuleFor(contact => contact.Email).NotEmpty().WithMessage(
                    localiser[ResourceMessageName.DisplayNameMustBeDefined].Value)
                .MaximumLength(120)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(updateContactRequest.Email), 120));

            RuleFor(contact => contact.DisplayName).NotEmpty().WithMessage(
                    localiser[ResourceMessageName.EMailMustBeDefined].Value)
                .MaximumLength(120)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(updateContactRequest.DisplayName), 120));

            RuleFor(contact => contact.Username)
                .NotEmpty()
                .WithMessage(
                    localiser[ResourceMessageName.UserNameMustBeDefined].Value)
                .MaximumLength(12)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(updateContactRequest.Username), 30));


            RuleFor(contact => contact.Username).Must(AlreadyExist).WithMessage(localiser[ResourceMessageName.DoesNotExist].Value);
        }

        private bool AlreadyExist(string userName)
        {
            var result = _contactQueryRepository.CheckContactExists(userName).Result;
            return result;
        }
    }
}