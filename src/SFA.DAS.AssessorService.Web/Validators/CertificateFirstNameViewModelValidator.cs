﻿using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateFirstNameViewModelValidator : AbstractValidator<CertificateFirstNameViewModel>
    {
        public CertificateFirstNameViewModelValidator(IStringLocalizer<CertificateFirstNameViewModel> localizer)
        {
            RuleFor(vm => vm.FirstName).NotEmpty().WithMessage(localizer["NameCannotBeEmpty"]);
        }
    }
}
