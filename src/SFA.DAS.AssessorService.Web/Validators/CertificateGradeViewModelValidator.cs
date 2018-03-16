using FluentValidation;
using SFA.DAS.AssessorService.Web.Controllers;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateGradeViewModelValidator : AbstractValidator<CertificateGradeViewModel>
    {
        public CertificateGradeViewModelValidator()
        {
            RuleFor(vm => vm.SelectedGrade).NotEmpty().WithMessage("A grade should be selected.");
        }
    }



    //public class SearchQueryViewModelValidator : AbstractValidator<SearchViewModel>
    //{
    //    public SearchQueryViewModelValidator(IStringLocalizer<SearchQueryViewModelValidator> localizer)
    //    {
    //        RuleFor(x => x.Surname).NotEmpty().WithMessage(localizer[ResourceKey.LastNameShouldNotBeEmpty]);
    //        RuleFor(x => x.Uln)
    //            .NotEmpty().WithMessage(localizer[ResourceKey.UlnShouldNotBeEmpty])
    //            .Matches(@"^\d{10}$").WithMessage(localizer[ResourceKey.UlnFormatInvalid]);
    //    }
    //}

}