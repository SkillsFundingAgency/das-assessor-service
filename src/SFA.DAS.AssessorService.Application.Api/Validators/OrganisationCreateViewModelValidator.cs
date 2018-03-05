using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class OrganisationCreateViewModelValidator : AbstractValidator<CreateOrganisationRequest>
    {
        private readonly IContactQueryRepository _contactQueryRepository;     
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public OrganisationCreateViewModelValidator(IStringLocalizer<OrganisationCreateViewModelValidator> localiser,
            IContactQueryRepository contactQueryRepository,
            IOrganisationQueryRepository organisationQueryRepository
        )
        {            
            _contactQueryRepository = contactQueryRepository;
            _organisationQueryRepository = organisationQueryRepository;

            // ReSharper disable once LocalNameCapturedOnly
            CreateOrganisationRequest organisationCreateViewModel;

            RuleFor(organisation => organisation.EndPointAssessorOrganisationId).NotEmpty().WithMessage(
                localiser[ResourceMessageName.EndPointAssessorOrganisationIdMustBeDefined,
                    nameof(organisationCreateViewModel.EndPointAssessorOrganisationId)].Value);
            RuleFor(organisation => organisation.EndPointAssessorName).NotEmpty().WithMessage(
                localiser[ResourceMessageName.EndPointAssessorNameMustBeDefined,
                    nameof(organisationCreateViewModel.EndPointAssessorName)].Value);
            RuleFor(organisation => organisation.EndPointAssessorUkprn).InclusiveBetween(10000000, 99999999)
                .WithMessage(localiser[ResourceMessageName.InvalidUkprn,
                    nameof(organisationCreateViewModel.EndPointAssessorUkprn)].Value);

            RuleFor(organisation => organisation.PrimaryContact).Must(HaveAssociatedPrimaryContactInContacts)
                .WithMessage(localiser[ResourceMessageName.PrimaryContactDoesNotExist,
                    nameof(organisationCreateViewModel.PrimaryContact)].Value);
            RuleFor(organisation => organisation.EndPointAssessorOrganisationId).Must(AlreadyExists).WithMessage(
                localiser[ResourceMessageName.AlreadyExists,
                    nameof(organisationCreateViewModel.EndPointAssessorOrganisationId)].Value);
        }

        private bool HaveAssociatedPrimaryContactInContacts(string primaryContact)
        {
            if (string.IsNullOrEmpty(primaryContact))
                return true;

            var result = _contactQueryRepository.CheckContactExists(primaryContact).Result;
            return result;
        }

        private bool AlreadyExists(string endPointAssessorOrganisationId)
        {
            return !_organisationQueryRepository.CheckIfAlreadyExists(endPointAssessorOrganisationId).Result;
        }
    }
}