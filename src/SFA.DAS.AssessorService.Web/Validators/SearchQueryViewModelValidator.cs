using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class SearchQueryViewModelValidator : AbstractValidator<SearchViewModel>
    {
        public SearchQueryViewModelValidator(IStringLocalizer<SearchQueryViewModelValidator> localizer)
        {
            RuleFor(x => x.Surname).NotEmpty().WithMessage(localizer[ResourceKey.LastNameShouldNotBeEmpty]);
            RuleFor(x => x.Uln).NotEmpty().Matches(@"^\d{10}$").WithMessage(localizer[ResourceKey.UlnFormatInvalid]);
        }
    }
}