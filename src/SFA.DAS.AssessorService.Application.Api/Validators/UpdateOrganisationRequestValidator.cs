using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class UpdateOrganisationRequestValidator : AbstractValidator<UpdateOrganisationRequest>
    {
        private readonly IContactQueryRepository _contactQueryRepository;       
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public UpdateOrganisationRequestValidator(IStringLocalizer<UpdateOrganisationRequestValidator> localiser,
            IContactQueryRepository contactQueryRepository,
            IOrganisationQueryRepository organisationQueryRepository
        )
        {          
            _contactQueryRepository = contactQueryRepository;
            _organisationQueryRepository = organisationQueryRepository;

            // ReSharper disable once LocalNameCapturedOnly
            UpdateOrganisationRequest updateOrganisationRequest;

            RuleFor(organisation => organisation.EndPointAssessorName).NotEmpty().WithMessage(
                string.Format(localiser[ResourceMessageName.MustBeDefined].Value,
                    nameof(updateOrganisationRequest.EndPointAssessorName).ToCamelCase()));

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
                    if (!result)
                    {
                        context.AddFailure(new ValidationFailure("Organisation",
                            string.Format(localiser[ResourceMessageName.DoesNotExist].Value, "Organisation", endPointAssessorOrganisationId)));
                    }
                });
        }          
    }
}