using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class CreateOrganisationRequestValidator : AbstractValidator<CreateOrganisationRequest>
    {
        private readonly IContactQueryRepository _contactQueryRepository;     
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public CreateOrganisationRequestValidator(IStringLocalizer<CreateOrganisationRequestValidator> localiser,
            IContactQueryRepository contactQueryRepository,
            IOrganisationQueryRepository organisationQueryRepository
        )
        {            
            _contactQueryRepository = contactQueryRepository;
            _organisationQueryRepository = organisationQueryRepository;
          
            // ReSharper disable once LocalNameCapturedOnly
            CreateOrganisationRequest createOrganisationRequest;         

            RuleFor(organisation => organisation.EndPointAssessorOrganisationId)
                .NotEmpty()
                .WithMessage(
                    localiser[ResourceMessageName.EndPointAssessorOrganisationIdMustBeDefined].Value)
                .MaximumLength(12)
                // Please note we have to string.Format this due to limitation in Moq not handling Optional
                // Params
                .WithMessage(string.Format(localiser[ResourceMessageName.MaxLengthError].Value,
                    nameof(createOrganisationRequest.EndPointAssessorOrganisationId), 12));

            RuleFor(organisation => organisation.EndPointAssessorName).NotEmpty().WithMessage(
                localiser[ResourceMessageName.EndPointAssessorNameMustBeDefined].Value);

            RuleFor(organisation => organisation.EndPointAssessorUkprn).InclusiveBetween(10000000, 99999999)
                .WithMessage(localiser[ResourceMessageName.InvalidUkprn].Value);

            RuleFor(organisation => organisation.PrimaryContact).Must(PrimaryContactMustExist)
                .WithMessage(localiser[ResourceMessageName.PrimaryContactDoesNotExist].Value);              
           
            RuleFor(organisation => organisation.EndPointAssessorOrganisationId).Must(NotAlreadyExist).WithMessage(
                localiser[ResourceMessageName.AlreadyExists].Value);
        }

        private bool PrimaryContactMustExist(string primaryContact)
        {
            if (string.IsNullOrEmpty(primaryContact))
                return true;

            var result = _contactQueryRepository.CheckContactExists(primaryContact).Result;
            return result;
        }

        private bool NotAlreadyExist(string endPointAssessorOrganisationId)
        {
            return !_organisationQueryRepository.CheckIfAlreadyExists(endPointAssessorOrganisationId).Result;
        }
    }
}