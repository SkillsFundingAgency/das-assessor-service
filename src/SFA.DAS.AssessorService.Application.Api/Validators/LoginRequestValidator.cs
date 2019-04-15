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
            RuleFor(lr => lr.DisplayName).NotEmpty().WithMessage("Display name must not be empty");
            RuleFor(lr => lr.Email).NotEmpty().WithMessage("Email must not be empty");
            RuleFor(lr => lr.SignInId).NotEmpty().WithMessage("SignInId must not be empty");
        }
    }
}