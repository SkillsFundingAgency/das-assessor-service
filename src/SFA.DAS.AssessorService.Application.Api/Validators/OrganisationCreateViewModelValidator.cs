namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    using FluentValidation;
    using Microsoft.Extensions.Localization;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class OrganisationCreateViewModelValidator : AbstractValidator<OrganisationCreateViewModel>
    {
        private readonly IStringLocalizer<OrganisationCreateViewModelValidator> _localizer;
        private readonly IContactRepository _contactRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public OrganisationCreateViewModelValidator(IStringLocalizer<OrganisationCreateViewModelValidator> localizer,
              IContactRepository contactRepository,
              IOrganisationQueryRepository organisationQueryRepository
            ) : base()
        {
            _localizer = localizer;
            _contactRepository = contactRepository;
            _organisationQueryRepository = organisationQueryRepository;

            var organisationCreateViewModel = new OrganisationCreateViewModel();

            RuleFor(organisation => organisation.EndPointAssessorOrganisationId).NotEmpty().WithMessage(_localizer[ResourceMessageName.EndPointAssessorOrganisationIdMustBeDefined, nameof(organisationCreateViewModel.EndPointAssessorOrganisationId)].Value);
            RuleFor(organisation => organisation.EndPointAssessorName).NotEmpty().WithMessage(_localizer[ResourceMessageName.EndPointAssessorNameMustBeDefined, nameof(organisationCreateViewModel.EndPointAssessorName)].Value);
            RuleFor(organisation => organisation.EndPointAssessorUKPRN).InclusiveBetween(10000000, 99999999).WithMessage(_localizer[ResourceMessageName.InvalidUKPRN, nameof(organisationCreateViewModel.EndPointAssessorUKPRN)].Value);

            RuleFor(customer => customer.PrimaryContactId).Must(HaveAssociatedPrimaryContactInContacts).WithMessage(_localizer[ResourceMessageName.PrimaryContactDoesNotExist, nameof(organisationCreateViewModel.PrimaryContactId)].Value);
            RuleFor(customer => customer.EndPointAssessorOrganisationId).Must(AlreadyExists).WithMessage(_localizer[ResourceMessageName.AlreadyExists, nameof(organisationCreateViewModel.EndPointAssessorOrganisationId)].Value);
        }

        private bool AlreadyExists(string endPointAssessorOrganisationId)
        {
            return !_organisationQueryRepository.CheckIfAlreadyExists(endPointAssessorOrganisationId).Result;
        }

        private bool HaveAssociatedPrimaryContactInContacts(int? primaryContactId)
        {
            if (!primaryContactId.HasValue)
                return true;

            var result = _contactRepository.CheckContactExists(primaryContactId.Value).Result;
            return result;
        }
    }
}
