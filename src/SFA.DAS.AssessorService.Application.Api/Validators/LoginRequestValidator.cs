﻿using System.Linq;
using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator(IStringLocalizer<LoginRequestValidator> localizer)
        {
            RuleFor(lr => lr.GovUkIdentifier).NotEmpty().WithMessage("GovIdentifier must not be empty");
        }
    }
}