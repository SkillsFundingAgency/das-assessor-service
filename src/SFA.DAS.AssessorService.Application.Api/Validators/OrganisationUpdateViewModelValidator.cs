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
        private readonly IContactRepository _contactRepository;
        private readonly IOrganisationRepository _organisationRepository;

        public OrganisationUpdateViewModelValidator(IStringLocalizer<OrganisationUpdateViewModelValidator> localizer,
              IContactRepository contactRepository,
              IOrganisationRepository organisationRepository
            ) : base()
        {
            _localizer = localizer;
            _contactRepository = contactRepository;
            _organisationRepository = organisationRepository;

            var organisationUpdateViewModel = new OrganisationUpdateViewModel();

            RuleFor(organisation => organisation.Id).NotEmpty().WithMessage(_localizer[ResourceMessageName.IdMustExist, nameof(organisationUpdateViewModel.Id)].Value);
            RuleFor(organisation => organisation.EndPointAssessorName).NotEmpty().WithMessage(_localizer[ResourceMessageName.EndPointAssessorNameMustBeDefined, nameof(organisationUpdateViewModel.EndPointAssessorName)].Value);
            RuleFor(organisation => organisation.PrimaryContactId).Must(HaveAssociatedPrimaryContactInContacts).WithMessage(_localizer[ResourceMessageName.PrimaryContactDoesNotExist, nameof(organisationUpdateViewModel.PrimaryContactId)].Value);     
            RuleFor(organisation => organisation.Id).Must(AlreadyExist).WithMessage(_localizer[ResourceMessageName.DoesNotExist, nameof(organisationUpdateViewModel.Id)].Value);
        }

        private bool AlreadyExist(Guid id)
        {
            return _organisationRepository.CheckIfAlreadyExists(id).Result;
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
