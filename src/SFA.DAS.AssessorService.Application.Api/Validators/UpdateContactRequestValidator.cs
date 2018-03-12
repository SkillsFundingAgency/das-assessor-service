using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Text.RegularExpressions;

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

            RuleFor(contact => contact.Email)
                .Custom((email, context) =>
                {
                    if (string.IsNullOrEmpty(email))
                        return;

                    if (email.Length > 120)
                    {
                        context.AddFailure(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                            nameof(updateContactRequest.Email).ToCamelCase(), 120));
                    }

                    var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    var match = regex.Match(email);
                    if (!match.Success)
                    {
                        context.AddFailure(string.Format(localiser[ResourceMessageName.MustBeValidEmailAddress]
                            .Value));
                    }
                });

            RuleFor(contact => contact.DisplayName)
                .Custom((displayName, context) =>
                {
                    if (string.IsNullOrEmpty(displayName))
                        return;

                    if (displayName.Length > 120)
                    {
                        context.AddFailure(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                            nameof(updateContactRequest.DisplayName).ToCamelCase(), 120));
                    }
                });


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
                    if (string.IsNullOrEmpty(contact.UserName))
                        return;

                    var result = contactQueryRepository.CheckContactExists(contact.UserName).Result;
                    if (!result)
                    {
                        context.AddFailure(new ValidationFailure("Contact",
                            string.Format(localiser[ResourceMessageName.DoesNotExist].Value, "Contact",
                                contact.UserName)));
                    }
                });
        }
    }
}