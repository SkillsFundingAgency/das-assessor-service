﻿using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class ChooseStandardViewModelValidator  : AbstractValidator<ChooseStandardViewModel>
    {
        public ChooseStandardViewModelValidator()
        {
            RuleFor(vm => vm.StdCode).NotEmpty().WithMessage("Select a Standard");
        }
    }
}