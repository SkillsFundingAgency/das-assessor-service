using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class CreateContactRequestValidator : AbstractValidator<CreateContactRequest>
    {
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
 
        public CreateContactRequestValidator(IStringLocalizer<CreateContactRequestValidator> localiser,
            IOrganisationQueryRepository organisationQueryRepository,
            IContactQueryRepository contactQueryRepository
        )
        {
            _organisationQueryRepository = organisationQueryRepository;
            _contactQueryRepository = contactQueryRepository;

            // ReSharper disable once LocalNameCapturedOnly
            CreateContactRequest createContactRequest;
            RuleFor(contact => contact.Email).NotEmpty().WithMessage(
                   localiser[ResourceMessageName.DisplayNameMustBeDefined].Value)
                .MaximumLength(120)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(createContactRequest.Email), 120));

            RuleFor(contact => contact.DisplayName).NotEmpty().WithMessage(
                localiser[ResourceMessageName.EMailMustBeDefined].Value)
                .MaximumLength(120)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(createContactRequest.DisplayName), 120));

            RuleFor(contact => contact.Username)
                .NotEmpty()
                .WithMessage(
                       localiser[ResourceMessageName.UserNameMustBeDefined].Value)
               .MaximumLength(12)
               // Please note we have to string.Format this due to limitation in Moq not handling Optional
               // Params
               .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(createContactRequest.Username), 30));

            RuleFor(contact => contact.EndPointAssessorOrganisationId).NotEmpty().WithMessage(
                localiser[ResourceMessageName.EndPointAssessorOrganisationIdMustBeDefined].Value);

            RuleFor(contact => contact).Must(NotAlreadyExist).WithMessage(localiser[ResourceMessageName.AlreadyExists].Value);

            RuleFor(contact => contact.EndPointAssessorOrganisationId).Must(HaveExistingOrganisation).WithMessage(
                localiser[ResourceMessageName.HaveExistingOrganisation].Value);
        }

        private bool NotAlreadyExist(CreateContactRequest contact)
        {
            var result = _contactQueryRepository.CheckContactExists(contact.Username).Result;
            return !result;
        }

        private bool HaveExistingOrganisation(string endPointAssessorOrganisationId)
        {
            var result = _organisationQueryRepository.CheckIfAlreadyExists(endPointAssessorOrganisationId).Result;
            return result;
        }
    }
}