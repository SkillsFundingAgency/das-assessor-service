using System;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateDateViewModelValidator : AbstractValidator<CertificateDateViewModel>
    {
        public CertificateDateViewModelValidator(IStringLocalizer<CertificateDateViewModelValidator> localizer)
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (int.TryParse(vm.Day, out var day) && int.TryParse(vm.Month, out var month) && int.TryParse(vm.Year, out var year))
                {
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
                }
                else
                {
                    context.AddFailure("Date", localizer["IncorrectFormat"]);
                }
            });
            RuleFor(vm => vm).Must(BeAtLeastTwelveMonthsFromStartDate).WithName("AchievementDate").WithSeverity(Severity.Warning)
                .WithMessage("Date must be at least 12 months greater than the Start Date");
        }

        private bool BeAtLeastTwelveMonthsFromStartDate(CertificateDateViewModel vm)
        {
            if (int.TryParse(vm.Day, out var day) && int.TryParse(vm.Month, out var month) &&
                int.TryParse(vm.Year, out var year))
            {
                var achievementDate = new DateTime(year, month, day);
                if (achievementDate < vm.StartDate.AddMonths(12) && vm.WarningShown == "false")
                {
                    return false;
                }
            }

            return true;
        }
    }
}