namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    using FluentValidation;
    using Microsoft.Extensions.Localization;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class ContactCreateViewModelValidator : AbstractValidator<ContactCreateViewModel>
    {
        private readonly IStringLocalizer<ContactCreateViewModelValidator> _localizer;

        public ContactCreateViewModelValidator(IStringLocalizer<ContactCreateViewModelValidator> localizer
            ) : base()
        {
            _localizer = localizer;

            var organisationCreateViewModel = new ContactCreateViewModel();
            RuleFor(organisation => organisation.ContactEmail).NotEmpty().WithMessage(_localizer[ResourceMessageName.ContactNameMustBeDefined, nameof(organisationCreateViewModel.ContactName)].Value);
            RuleFor(organisation => organisation.ContactName).NotEmpty().WithMessage(_localizer[ResourceMessageName.ContactEMailMustBeDefined, nameof(organisationCreateViewModel.ContactEmail)].Value);
            RuleFor(organisation => organisation.EndPointAssessorContactId).NotEmpty().WithMessage(_localizer[ResourceMessageName.EndPointAssessorContactIdMustBeDefined, nameof(organisationCreateViewModel.EndPointAssessorContactId)].Value);
        }
    }
}
