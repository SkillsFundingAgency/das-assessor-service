using System;
using FluentValidation;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class CertificateDateViewModelValidator : AbstractValidator<CertificateDateViewModel>
    {
        public CertificateDateViewModelValidator()
        {
            RuleFor(vm => vm.Day).NotEmpty().WithMessage("Enter the achievement day").DependentRules(() =>
            {
                RuleFor(vm => vm.Day).Must(BeANumber).WithMessage("Day must be a number").DependentRules(() =>
                {
                    RuleFor(vm => vm.Day).Must(BeInValidDayRange).WithMessage("Enter a valid day");
                });
            });

            RuleFor(vm => vm.Month).NotEmpty().WithMessage("Enter the achievement month").DependentRules(() =>
            {
                RuleFor(vm => vm.Month).Must(BeANumber).WithMessage("Month must be a number").DependentRules(() =>
                {
                    RuleFor(vm => vm.Month).Must(BeInValidMonthRange).WithMessage("Enter a valid month");
                });
            });

            RuleFor(vm => vm.Year).NotEmpty().WithMessage("Enter a valid month").DependentRules(() =>
            {
                RuleFor(vm => vm.Year).Length(4).WithMessage("Enter a valid year").DependentRules(() =>
                {
                    RuleFor(vm => vm.Year).Must(BeANumber).WithMessage("Year must be a number");
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
                        context.AddFailure("Date", "An achievement date cannot be in the future");
                    }

                    var achievementStartDate = new DateTime(2017, 1, 1);
                    if (achievementDate < achievementStartDate)
                    {
                        context.AddFailure("Date", "An achievement date cannot be before 01 01 2017");
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    context.AddFailure("Date", "Enter a valid date");
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
    }
}