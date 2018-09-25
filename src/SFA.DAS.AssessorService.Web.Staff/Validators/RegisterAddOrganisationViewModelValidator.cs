using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class RegisterAddOrganisationViewModelValidator : AbstractValidator<RegisterAddOrganisationViewModel>
    {
        public RegisterAddOrganisationViewModelValidator()
        {
            RuleFor(vm => vm.Name).NotEmpty().WithMessage("Search string required");
        }
    }
}
