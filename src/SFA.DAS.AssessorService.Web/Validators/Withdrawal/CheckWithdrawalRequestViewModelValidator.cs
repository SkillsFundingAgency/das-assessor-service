﻿using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class CheckWithdrawalRequestViewModelValidator : AbstractValidator<CheckWithdrawalRequestViewModel>
    {
        public CheckWithdrawalRequestViewModelValidator()
        {
            RuleFor(vm => vm.Continue)
                .Cascade(CascadeMode.Stop) 
                .NotEmpty()
                .Must(value => value.ToLower() == "yes" || value.ToLower() == "no")
                .WithMessage("Select Yes or No");
        }
    }
}