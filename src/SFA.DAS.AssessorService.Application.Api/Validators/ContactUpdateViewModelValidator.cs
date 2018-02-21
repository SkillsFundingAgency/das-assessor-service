namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    using FluentValidation;
    using Microsoft.Extensions.Localization;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class ContactUpdateViewModelValidator : AbstractValidator<ContactUpdateViewModel>
    {
        private readonly IStringLocalizer<ContactUpdateViewModelValidator> _localizer;

        public ContactUpdateViewModelValidator(IStringLocalizer<ContactUpdateViewModelValidator> localizer
            ) : base()
        {
            _localizer = localizer;

            var organisationCreateViewModel = new ContactCreateViewModel();
            RuleFor(organisation => organisation.ContactEmail).NotEmpty().WithMessage(_localizer[ResourceMessageName.ContactNameMustBeDefined, nameof(organisationCreateViewModel.ContactName)].Value);
            RuleFor(organisation => organisation.ContactName).NotEmpty().WithMessage(_localizer[ResourceMessageName.ContactEMailMustBeDefined, nameof(organisationCreateViewModel.ContactEmail)].Value);            
        }
    }
}
