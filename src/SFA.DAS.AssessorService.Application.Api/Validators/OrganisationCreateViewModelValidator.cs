namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    using System;
    using AssessorService.Api.Types.Models;
    using Consts;
    using FluentValidation;
    using Interfaces;
    using Microsoft.Extensions.Localization;

    public class OrganisationCreateViewModelValidator : AbstractValidator<CreateOrganisationRequest>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IStringLocalizer<OrganisationCreateViewModelValidator> _localizer;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public OrganisationCreateViewModelValidator(IStringLocalizer<OrganisationCreateViewModelValidator> localizer,
            IContactQueryRepository contactQueryRepository,
            IOrganisationQueryRepository organisationQueryRepository
        )
        {
            _localizer = localizer;
            _contactQueryRepository = contactQueryRepository;
            _organisationQueryRepository = organisationQueryRepository;

            var organisationCreateViewModel = new CreateOrganisationRequest();

            RuleFor(organisation => organisation.EndPointAssessorOrganisationId).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.EndPointAssessorOrganisationIdMustBeDefined,
                    nameof(organisationCreateViewModel.EndPointAssessorOrganisationId)].Value);
            RuleFor(organisation => organisation.EndPointAssessorName).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.EndPointAssessorNameMustBeDefined,
                    nameof(organisationCreateViewModel.EndPointAssessorName)].Value);
            RuleFor(organisation => organisation.EndPointAssessorUKPRN).InclusiveBetween(10000000, 99999999)
                .WithMessage(_localizer[ResourceMessageName.InvalidUKPRN,
                    nameof(organisationCreateViewModel.EndPointAssessorUKPRN)].Value);

            RuleFor(organisation => organisation.PrimaryContactId).Must(HaveAssociatedPrimaryContactInContacts)
                .WithMessage(_localizer[ResourceMessageName.PrimaryContactDoesNotExist,
                    nameof(organisationCreateViewModel.PrimaryContactId)].Value);
            RuleFor(organisation => organisation.EndPointAssessorOrganisationId).Must(AlreadyExists).WithMessage(
                _localizer[ResourceMessageName.AlreadyExists,
                    nameof(organisationCreateViewModel.EndPointAssessorOrganisationId)].Value);
        }

        private bool HaveAssociatedPrimaryContactInContacts(Guid? primaryContactId)
        {
            if (!primaryContactId.HasValue)
                return true;

            var result = _contactQueryRepository.CheckContactExists(primaryContactId.Value).Result;
            return result;
        }

        private bool AlreadyExists(string endPointAssessorOrganisationId)
        {
            return !_organisationQueryRepository.CheckIfAlreadyExists(endPointAssessorOrganisationId).Result;
        }
    }
}