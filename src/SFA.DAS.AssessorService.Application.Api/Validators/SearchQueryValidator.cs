using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class SearchQueryValidator : AbstractValidator<SearchQuery>
    {
        public SearchQueryValidator(IStringLocalizer<SearchQueryValidator> localizer)
        {
            RuleFor(query => query.Surname).NotEmpty().WithMessage(localizer[ResourceMessageName.MustHaveSurname]);
            RuleFor(query => query.UkPrn).InclusiveBetween(10000000, 99999999)
                .WithMessage(localizer[ResourceMessageName.InvalidUkprn].Value);
            RuleFor(query => query.Uln).InclusiveBetween(1111111111,9999999999)
                .WithMessage(localizer[ResourceMessageName.InvalidUln].Value);
            RuleFor(query => query.Username).NotEmpty()
                .WithMessage(localizer[ResourceMessageName.MustHaveUsername].Value);
        }
    }
}