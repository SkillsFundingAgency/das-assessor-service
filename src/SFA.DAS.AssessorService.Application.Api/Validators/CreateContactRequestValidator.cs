using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class CreateContactRequestValidator : AbstractValidator<CreateContactRequest>
    {
        public CreateContactRequestValidator(IStringLocalizer<CreateContactRequestValidator> localiser,
            IOrganisationQueryRepository organisationQueryRepository,
            IContactQueryRepository contactQueryRepository
        )
        {
            // ReSharper disable once LocalNameCapturedOnly
            CreateContactRequest createContactRequest;
            RuleFor(contact => contact.Email).NotEmpty().WithMessage(
                    string.Format(localiser[ResourceMessageName.MustBeDefined].Value,
                        nameof(createContactRequest.Email).ToCamelCase()))
                .MaximumLength(120)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(createContactRequest.Email), 120))
                .EmailAddress()
                .WithMessage(string.Format(localiser[ResourceMessageName.MustBeValidEmailAddress].Value));

            RuleFor(contact => contact.DisplayName).NotEmpty().WithMessage(
                    string.Format(localiser[ResourceMessageName.MustBeDefined].Value, nameof(createContactRequest.DisplayName).ToCamelCase()))
                .MaximumLength(120)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(createContactRequest.DisplayName), 120));

            RuleFor(contact => contact.UserName)
                .NotEmpty()
                .WithMessage(
                    string.Format(localiser[ResourceMessageName.MustBeDefined].Value, nameof(createContactRequest.UserName).ToCamelCase()))
                .MaximumLength(30)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(createContactRequest.UserName), 30));

            RuleFor(contact => contact)
                .Custom((contact, context) =>
                {
                    var result = contactQueryRepository.CheckContactExists(contact.UserName).Result;
                    if (result)
                    {
                        context.AddFailure(new ValidationFailure("Contact",
                            string.Format(localiser[ResourceMessageName.AlreadyExists].Value, "Contact")));
                    }
                });

            RuleFor(contact => contact.EndPointAssessorOrganisationId)
                .Custom((endPointAssessorOrganisationId, context) =>
                {
                    if (string.IsNullOrEmpty(endPointAssessorOrganisationId))
                    {
                        context.AddFailure(new ValidationFailure(nameof(endPointAssessorOrganisationId),
                            string.Format(localiser[ResourceMessageName.MustBeDefined].Value,
                                nameof(createContactRequest.EndPointAssessorOrganisationId))));
                        return;
                    }

                    var result = organisationQueryRepository.CheckIfAlreadyExists(endPointAssessorOrganisationId).Result;
                    if (!result)
                    {
                        context.AddFailure(new ValidationFailure("Organisation",
                           string.Format(localiser[ResourceMessageName.DoesNotExist].Value, "Organisation", endPointAssessorOrganisationId)));
                    }
                });
        }
    }
}