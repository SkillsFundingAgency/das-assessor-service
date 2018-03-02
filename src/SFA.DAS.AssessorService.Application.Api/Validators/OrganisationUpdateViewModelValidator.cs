namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    using System;
    using AssessorService.Api.Types.Models;
    using Consts;
    using FluentValidation;
    using Interfaces;
    using Microsoft.Extensions.Localization;

    public class OrganisationUpdateViewModelValidator : AbstractValidator<UpdateOrganisationRequest>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IStringLocalizer<OrganisationUpdateViewModelValidator> _localizer;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public OrganisationUpdateViewModelValidator(IStringLocalizer<OrganisationUpdateViewModelValidator> localizer,
            IContactQueryRepository contactQueryRepository,
            IOrganisationQueryRepository organisationQueryRepository
        )
        {
            _localizer = localizer;
            _contactQueryRepository = contactQueryRepository;
            _organisationQueryRepository = organisationQueryRepository;

            // ReSharper disable once LocalNameCapturedOnly
            UpdateOrganisationRequest organisationUpdateViewModel;

            RuleFor(organisation => organisation.EndPointAssessorName).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.EndPointAssessorNameMustBeDefined,
                    nameof(organisationUpdateViewModel.EndPointAssessorName)].Value);
            RuleFor(organisation => organisation.PrimaryContact).Must(HaveAssociatedPrimaryContactInContacts)
                .WithMessage(_localizer[ResourceMessageName.PrimaryContactDoesNotExist,
                    nameof(organisationUpdateViewModel.PrimaryContact)].Value);
            RuleFor(organisation => organisation.EndPointAssessorOrganisationId).Must(AlreadyExist).WithMessage(
                _localizer[ResourceMessageName.DoesNotExist, nameof(organisationUpdateViewModel.EndPointAssessorOrganisationId)].Value);
        }

        private bool AlreadyExist(string endPointAssessorOrganisationId)
        {
            var result = _organisationQueryRepository.CheckIfAlreadyExists(endPointAssessorOrganisationId).Result;
            return result;
        }

        private bool HaveAssociatedPrimaryContactInContacts(string primaryContact)
        {
            if (string.IsNullOrEmpty(primaryContact))
                return true;

            var result = _contactQueryRepository.CheckContactExists(primaryContact).Result;
            return result;
        }
    }
}