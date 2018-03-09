using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class CreateOrganisationRequestValidator : AbstractValidator<CreateOrganisationRequest>
    {
        private readonly IContactQueryRepository _contactQueryRepository;

        public CreateOrganisationRequestValidator(IStringLocalizer<CreateOrganisationRequestValidator> localiser,
            IContactQueryRepository contactQueryRepository,
            IOrganisationQueryRepository organisationQueryRepository
        )
        {
            _contactQueryRepository = contactQueryRepository;

            // ReSharper disable once LocalNameCapturedOnly
            CreateOrganisationRequest createOrganisationRequest;

            RuleFor(organisation => organisation.EndPointAssessorOrganisationId)
                .NotEmpty()
                .WithMessage(
                    string.Format(localiser[ResourceMessageName.MustBeDefined].Value, nameof(createOrganisationRequest.EndPointAssessorOrganisationId).ToCamelCase()))
                .MaximumLength(12)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(createOrganisationRequest.EndPointAssessorOrganisationId), 12));

            RuleFor(organisation => organisation.EndPointAssessorName).NotEmpty().WithMessage(
                string.Format(localiser[ResourceMessageName.MustBeDefined].Value,
                    nameof(createOrganisationRequest.EndPointAssessorName).ToCamelCase()));

            RuleFor(organisation => organisation.EndPointAssessorUkprn).InclusiveBetween(10000000, 99999999)
                .WithMessage(localiser[ResourceMessageName.InvalidUkprn].Value);

            RuleFor(organisation => organisation.PrimaryContact)
                .Custom((primaryContact, context) =>
                {
                    if (string.IsNullOrEmpty(primaryContact))
                        return;

                    var result = contactQueryRepository.CheckContactExists(primaryContact).Result;
                    if (!result)
                    {
                        context.AddFailure(new ValidationFailure("PrimaryContact",
                            string.Format(localiser[ResourceMessageName.DoesNotExist].Value, "PrimaryContact", primaryContact)));
                    }
                });

            RuleFor(organisation => organisation.EndPointAssessorOrganisationId)
                .Custom((endPointAssessorOrganisationId, context) =>
                {
                    var result = organisationQueryRepository.CheckIfAlreadyExists(endPointAssessorOrganisationId).Result;
                    if (result)
                    {
                        context.AddFailure(new ValidationFailure("Organisation",
                            string.Format(localiser[ResourceMessageName.AlreadyExists].Value, "Organisation")));
                    }
                });
        }
    }
}