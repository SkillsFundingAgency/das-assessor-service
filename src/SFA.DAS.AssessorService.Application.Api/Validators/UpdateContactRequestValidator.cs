using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Extensions;
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
                    string.Format(localiser[ResourceMessageName.MustBeDefined].Value, nameof(updateContactRequest.Email).ToCamelCase()))
                .MaximumLength(120)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(updateContactRequest.Email), 120));

            RuleFor(contact => contact.DisplayName).NotEmpty().WithMessage(
                    string.Format(localiser[ResourceMessageName.MustBeDefined].Value, nameof(updateContactRequest.DisplayName).ToCamelCase()))
                .MaximumLength(120)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(updateContactRequest.DisplayName), 120));

            RuleFor(contact => contact.UserName)
                .NotEmpty()
                .WithMessage(
                    string.Format(localiser[ResourceMessageName.MustBeDefined].Value, nameof(updateContactRequest.UserName).ToCamelCase()))
                .MaximumLength(30)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(updateContactRequest.UserName), 30));


            RuleFor(contact => contact)
                .Custom((contact, context) =>
                {
                    var result = contactQueryRepository.CheckContactExists(contact.UserName).Result;
                    if (!result)
                    {
                        context.AddFailure(new ValidationFailure("Contact",
                            string.Format(localiser[ResourceMessageName.DoesNotExist].Value, "Contact", contact.UserName)));
                    }
                });
        }        
    }
}