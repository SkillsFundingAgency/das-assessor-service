namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    using FluentValidation;
    using Microsoft.Extensions.Localization;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System;

    public class OrganisationUpdateViewModelValidator : AbstractValidator<OrganisationUpdateViewModel>
    {
        private readonly IStringLocalizer<OrganisationUpdateViewModelValidator> _localizer;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public OrganisationUpdateViewModelValidator(IStringLocalizer<OrganisationUpdateViewModelValidator> localizer,
              IContactQueryRepository contactQueryRepository,
              IOrganisationQueryRepository organisationQueryRepository
            ) : base()
        {
            _localizer = localizer;
            _contactQueryRepository = contactQueryRepository;
            _organisationQueryRepository = organisationQueryRepository;

            var organisationUpdateViewModel = new OrganisationUpdateViewModel();
          
            RuleFor(organisation => organisation.EndPointAssessorName).NotEmpty().WithMessage(_localizer[ResourceMessageName.EndPointAssessorNameMustBeDefined, nameof(organisationUpdateViewModel.EndPointAssessorName)].Value);
            RuleFor(organisation => organisation.PrimaryContactId).Must(HaveAssociatedPrimaryContactInContacts).WithMessage(_localizer[ResourceMessageName.PrimaryContactDoesNotExist, nameof(organisationUpdateViewModel.PrimaryContactId)].Value);     
            RuleFor(organisation => organisation.Id).Must(AlreadyExist).WithMessage(_localizer[ResourceMessageName.DoesNotExist, nameof(organisationUpdateViewModel.Id)].Value);
        }

        private bool AlreadyExist(Guid id)
        {
            var result = _organisationQueryRepository.CheckIfAlreadyExists(id).Result;
            return result;
        }

        private bool HaveAssociatedPrimaryContactInContacts(Guid? primaryContactId)
        {
            if (!primaryContactId.HasValue)
                return true;

            var result = _contactQueryRepository.CheckContactExists(primaryContactId.Value).Result;
            return result;
        }
    }
}
