using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Web.Constants;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class SearchQueryViewModelValidator : AbstractValidator<SearchQueryViewModel>
    {
        private readonly IStringLocalizer<SearchQueryViewModelValidator> _localizer;

        public SearchQueryViewModelValidator(IStringLocalizer<SearchQueryViewModelValidator> localizer)
        {
            _localizer = localizer;
            RuleFor(x => x.Surname).NotEmpty().WithMessage(_localizer[ResourceKey.LastNameShouldNotBeEmpty]);
            RuleFor(x => x.Uln).NotEmpty().WithMessage(_localizer[ResourceKey.UlnShouldNotBeEmpty]);
        }
    }
}