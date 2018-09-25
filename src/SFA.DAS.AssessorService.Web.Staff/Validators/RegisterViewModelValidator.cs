using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(vm => vm.SearchString).NotEmpty().WithMessage("Search string required")
                .Must(x => x?.Trim().Length > 1)
                .WithMessage("The expression entered is too short. Please enter 2 or more letters.");

        }
    }
}
