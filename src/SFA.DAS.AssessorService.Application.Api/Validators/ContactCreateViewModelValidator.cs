namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    using FluentValidation;
    using Microsoft.Extensions.Localization;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class ContactCreateViewModelValidator : AbstractValidator<ContactCreateViewModel>
    {
        private readonly IStringLocalizer<OrganisationCreateViewModelValidator> _localizer;

        public ContactCreateViewModelValidator(IStringLocalizer<OrganisationCreateViewModelValidator> localizer
            ) : base()
        {
            _localizer = localizer;

            var organisationCreateViewModel = new OrganisationCreateViewModel();
            RuleFor(organisation => organisation.EndPointAssessorUKPRN).InclusiveBetween(10000000, 99999999).WithMessage(_localizer[ResourceMessageName.InvalidUKPRN, nameof(organisationCreateViewModel.EndPointAssessorUKPRN)].Value);
        }
    }
}
