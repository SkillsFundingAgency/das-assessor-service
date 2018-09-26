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
            RuleFor(vm => vm.SearchString).NotEmpty().WithMessage("Enter 2 or more characters")
                .Must(x => x?.Trim().Length > 1)
                .WithMessage("Enter 2 or more characters");

        }
    }
}
