using System;
using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateDateViewModelValidator : AbstractValidator<CertificateDateViewModel>
    {
        public CertificateDateViewModelValidator(IStringLocalizer<CertificateDateViewModelValidator> localizer)
        {
            RuleFor(vm => vm.Day).NotEmpty().WithMessage(localizer["DayRequired"]).DependentRules(() =>
            {
                RuleFor(vm => vm.Day).Must(BeANumber).WithMessage(localizer["DayNumber"]).DependentRules(() =>
                {
                    RuleFor(vm => vm.Day).Must(BeInValidDayRange).WithMessage(localizer["DayRange"]);
                });
            });

            RuleFor(vm => vm.Month).NotEmpty().WithMessage(localizer["MonthRequired"]).DependentRules(() =>
            {
                RuleFor(vm => vm.Month).Must(BeANumber).WithMessage(localizer["MonthNumber"]).DependentRules(() =>
                {
                    RuleFor(vm => vm.Month).Must(BeInValidMonthRange).WithMessage(localizer["MonthRange"]);
                });
            });

            RuleFor(vm => vm.Year).NotEmpty().WithMessage(localizer["YearRequired"]).DependentRules(() =>
            {
                RuleFor(vm => vm.Year).Length(4).WithMessage(localizer["YearRange"]).DependentRules(() =>
                {
                    RuleFor(vm => vm.Year).Must(BeANumber).WithMessage(localizer["YearNumber"]);
                });
            });

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (!int.TryParse(vm.Day, out var day) || !int.TryParse(vm.Month, out var month) ||
                    !int.TryParse(vm.Year, out var year)) return;
                try
                {
                    var achievementDate = new DateTime(year, month, day);

                    if (achievementDate > SystemTime.UtcNow())
                    {
                        context.AddFailure("Date", localizer["DateMustNotBeInFuture"]);
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    context.AddFailure("Date", localizer["IncorrectFormat"]);
                }
            });
        }

        private bool BeInValidMonthRange(string monthString)
        {
            var month = int.Parse(monthString);
            return month >= 1 && month <= 12;
        }

        private bool BeInValidDayRange(string dayString)
        {
            var day = int.Parse(dayString);
            return day >= 1 && day <= 31;
        }


        private bool BeANumber(string datePart)
        {
            return int.TryParse(datePart, out int _);
        }

        //private bool BeAtLeastTwelveMonthsFromStartDate(CertificateDateViewModel vm)
        //{
        //    if (int.TryParse(vm.Day, out var day) && int.TryParse(vm.Month, out var month) &&
        //        int.TryParse(vm.Year, out var year))
        //    {
        //        var achievementDate = new DateTime(year, month, day);
        //        if (achievementDate < vm.StartDate.AddMonths(12) && vm.WarningShown == "false")
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}
    }
}