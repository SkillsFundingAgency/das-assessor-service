using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    /// <summary>
    /// Implements FluentValidation rules when handling UpdateEmailRequest.
    /// </summary>
    public class UpdateEmailRequestValidator : AbstractValidator<UpdateEmailRequest>
    {
        public UpdateEmailRequestValidator(
            IStringLocalizer<UpdateEmailRequestValidator> localiser,
            IContactQueryRepository contactQueryRepository
        )
        {
            var emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            RuleFor(contact => contact.Email)
                .Custom((email, context) =>
                {
                    
                    if (string.IsNullOrEmpty(email))
                        return;

                    if (email.Length > 120)
                    {
                        context.AddFailure(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                            nameof(UpdateEmailRequest.Email).ToCamelCase(), 120));
                    }

                    if (!emailRegex.Match(email).Success)
                    {
                        context.AddFailure(string.Format(localiser[ResourceMessageName.MustBeValidEmailAddress]
                            .Value));
                    }
                });

            RuleFor(contact => contact.NewEmail)
                .Custom((email, context) =>
                {
                    if (string.IsNullOrEmpty(email))
                        return;

                    if (email.Length > 120)
                    {
                        context.AddFailure(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                            nameof(UpdateEmailRequest.Email).ToCamelCase(), 120));
                    }

                    if (!emailRegex.Match(email).Success)
                    {
                        context.AddFailure(string.Format(localiser[ResourceMessageName.MustBeValidEmailAddress]
                            .Value));
                    }
                });


            RuleFor(contact => contact.UserName)
                .NotEmpty()
                .WithMessage(
                    string.Format(localiser[ResourceMessageName.MustBeDefined].Value, nameof(UpdateEmailRequest.UserName).ToCamelCase()))
                .MaximumLength(30)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(UpdateEmailRequest.UserName), 30));


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
