using FluentValidation;
using SFA.DAS.AssessorService.ViewModel.Models;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class SearchRequestValidator : AbstractValidator<SearchQueryViewModel>
    {
        public SearchRequestValidator()
        {

            When(query => query.SearchType == SearchTypes.Uln, () =>
                {
                    RuleFor(vm => vm.Uln)
                        .NotEmpty();
                });

            When(vm => vm.Uln == SearchTypes.DobSurname, () =>
            {
                RuleFor(vm => vm.DobDay).NotEmpty();
                RuleFor(vm => vm.DobMonth).NotEmpty();
                RuleFor(vm => vm.DobYear).NotEmpty();
                RuleFor(vm => vm.Surname).NotEmpty();
            });

            //RuleFor(organisation => organisation.EndPointAssessorName).NotEmpty().WithMessage(_localizer[ResourceMessageName.EndPointAssessorNameMustBeDefined, nameof(organisationUpdateViewModel.EndPointAssessorName)].Value);
        }
    }
}