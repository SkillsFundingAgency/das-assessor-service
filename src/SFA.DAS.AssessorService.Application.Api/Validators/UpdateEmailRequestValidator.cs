using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Validation;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    /// <summary>
    /// Implements FluentValidation rules when handling UpdateEmailRequest.
    /// </summary>
    public class UpdateEmailRequestValidator : AbstractValidator<UpdateEmailRequest>
    {
        public UpdateEmailRequestValidator(
            IStringLocalizer<UpdateEmailRequestValidator> localiser,
            IContactQueryRepository contactQueryRepository)
        {
            
            
            RuleFor(contact => contact.NewEmail)
                .MaximumLength(256)
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(UpdateEmailRequest.NewEmail), 256))
                .EmailRegexAddress()
                .WithMessage(string.Format(localiser[ResourceMessageName.MustBeValidEmailAddress].Value));

            

            RuleFor(contact => contact.GovUkIdentifier)
                .NotEmpty()
                .WithMessage(
                    string.Format(localiser[ResourceMessageName.MustBeDefined].Value, nameof(UpdateEmailRequest.GovUkIdentifier).ToCamelCase()));


            RuleFor(contact => contact)
                .Custom((contact, context) =>
                {
                    if (string.IsNullOrEmpty(contact.GovUkIdentifier))
                        return;

                    var result = contactQueryRepository.GetContactByGovIdentifier(contact.GovUkIdentifier).Result;
                    if (result == null)
                    {
                        context.AddFailure(new ValidationFailure("Contact",
                            string.Format(localiser[ResourceMessageName.DoesNotExist].Value, "Contact",
                                contact.GovUkIdentifier)));
                    }
                });
        }
    }
}
