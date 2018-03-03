using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class ContactUpdateViewModelValidator : AbstractValidator<UpdateContactRequest>
    {
        public ContactUpdateViewModelValidator(IStringLocalizer<ContactUpdateViewModelValidator> localiser)
        {
            // ReSharper disable once LocalNameCapturedOnly
            CreateContactRequest createContactRequest;
            RuleFor(organisation => organisation.Email).NotEmpty().WithMessage(
                localiser[ResourceMessageName.DisplayNameMustBeDefined,
                    nameof(createContactRequest.DisplayName)].Value);
            RuleFor(organisation => organisation.DisplayName).NotEmpty().WithMessage(
                localiser[ResourceMessageName.EMailMustBeDefined,
                    nameof(createContactRequest.Email)].Value);
            RuleFor(organisation => organisation.Username).NotEmpty().WithMessage(
                localiser[ResourceMessageName.UserNameMustBeDefined,
                    nameof(createContactRequest.Username)].Value);
        }
    }
}