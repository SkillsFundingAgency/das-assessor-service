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
        private readonly IOrganisationRepository _organisationRepository;

        public OrganisationCreateViewModelValidator(IStringLocalizer<OrganisationCreateViewModelValidator> localizer,
              IContactRepository contactRepository,
              IOrganisationRepository organisationRepository
            ) : base()
        {
            _localizer = localizer;
            _contactRepository = contactRepository;
            _organisationRepository = organisationRepository;

            var organisationCreateViewModel = new OrganisationCreateViewModel();

            RuleFor(customer => customer.EndPointAssessorOrganisationId).NotEmpty().WithMessage(_localizer[ResourceMessageName.EndPointAssessorOrganisationIdMustBeDefined, nameof(organisationCreateViewModel.EndPointAssessorOrganisationId)].Value);
            RuleFor(customer => customer.EndPointAssessorName).NotEmpty().WithMessage(_localizer[ResourceMessageName.EndPointAssessorNameMustBeDefined, nameof(organisationCreateViewModel.EndPointAssessorName)].Value);
            RuleFor(customer => customer.EndPointAssessorUKPRN).InclusiveBetween(10000000, 99999999).WithMessage(_localizer[ResourceMessageName.InvalidUKPRN, nameof(organisationCreateViewModel.EndPointAssessorUKPRN)].Value);
            
            RuleFor(customer => customer.PrimaryContactId).Must(HaveAssociatedPrimaryContactinContacts).WithMessage(_localizer[ResourceMessageName.PrimaryContactDoesNotExist, nameof(organisationCreateViewModel.PrimaryContactId)].Value);
            RuleFor(customer => customer.EndPointAssessorOrganisationId).Must(AlreadyExists).WithMessage(_localizer[ResourceMessageName.AlreadyExists, nameof(organisationCreateViewModel.EndPointAssessorOrganisationId)].Value);
        }

        private bool AlreadyExists(string endPointAssessorOrganisationId)
        {
            return  !_organisationRepository.CheckIfAlreadyExists(endPointAssessorOrganisationId).Result;
        }

        private bool HaveAssociatedPrimaryContactinContacts(int? primaryContactId)
        {
            if (!primaryContactId.HasValue)
                return true;

            var result = _contactRepository.CheckContactExists(primaryContactId.Value).Result;
            return result;
        }
    }
}
