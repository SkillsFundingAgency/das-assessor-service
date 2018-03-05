using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
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
            UpdateOrganisationRequest organisationUpdateViewModel;

            RuleFor(organisation => organisation.EndPointAssessorName).NotEmpty().WithMessage(
                localiser[ResourceMessageName.EndPointAssessorNameMustBeDefined,
                    nameof(organisationUpdateViewModel.EndPointAssessorName)].Value);
            RuleFor(organisation => organisation.PrimaryContact).Must(HaveAssociatedPrimaryContactInContacts)
                .WithMessage(localiser[ResourceMessageName.PrimaryContactDoesNotExist,
                    nameof(organisationUpdateViewModel.PrimaryContact)].Value);
            RuleFor(organisation => organisation.EndPointAssessorOrganisationId).Must(AlreadyExist).WithMessage(
                localiser[ResourceMessageName.DoesNotExist,
                    nameof(organisationUpdateViewModel.EndPointAssessorOrganisationId)].Value);
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