﻿using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Validation;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class NewContactValidator : AbstractValidator<CreateContactRequest>
    {
        public NewContactValidator(IStringLocalizer<CreateContactRequest> localizer)
        {
            RuleFor(vm => vm.Email)
                .EmailRegexAddress().WithMessage(localizer["Email must be valid"])
                .NotEmpty().WithMessage(localizer["Email must not be empty"]);

            RuleFor(vm => vm.FamilyName)
                .NotEmpty().WithMessage(localizer["Last name must not be empty"]);

            RuleFor(vm => vm.GivenName)
                .NotEmpty().WithMessage(localizer["First name must not be empty"]);
        }
    }
}
