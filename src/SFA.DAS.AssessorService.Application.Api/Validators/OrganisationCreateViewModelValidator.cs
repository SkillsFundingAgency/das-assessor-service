namespace SFA.DAS.AssessorService.Application.Api.Validators
{
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

            // ReSharper disable once LocalNameCapturedOnly
            CreateOrganisationRequest organisationCreateViewModel;

            RuleFor(organisation => organisation.EndPointAssessorOrganisationId).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.EndPointAssessorOrganisationIdMustBeDefined,
                    nameof(organisationCreateViewModel.EndPointAssessorOrganisationId)].Value);
            RuleFor(organisation => organisation.EndPointAssessorName).NotEmpty().WithMessage(
                _localizer[ResourceMessageName.EndPointAssessorNameMustBeDefined,
                    nameof(organisationCreateViewModel.EndPointAssessorName)].Value);
            RuleFor(organisation => organisation.EndPointAssessorUkprn).InclusiveBetween(10000000, 99999999)
                .WithMessage(_localizer[ResourceMessageName.InvalidUkprn,
                    nameof(organisationCreateViewModel.EndPointAssessorUkprn)].Value);

            RuleFor(organisation => organisation.PrimaryContact).Must(HaveAssociatedPrimaryContactInContacts)
                .WithMessage(_localizer[ResourceMessageName.PrimaryContactDoesNotExist,
                    nameof(organisationCreateViewModel.PrimaryContact)].Value);
            RuleFor(organisation => organisation.EndPointAssessorOrganisationId).Must(AlreadyExists).WithMessage(
                _localizer[ResourceMessageName.AlreadyExists,
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