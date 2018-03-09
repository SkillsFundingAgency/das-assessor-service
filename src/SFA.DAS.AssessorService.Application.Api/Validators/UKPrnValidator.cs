using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Application.Api.Consts;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class UkPrnValidator
    {
        private readonly IStringLocalizer<UkPrnValidator> _localizer;

        public UkPrnValidator(IStringLocalizer<UkPrnValidator> localizer)
        {
            _localizer = localizer;
        }

        public ValidationResult Validate(int ukprn)
        {
            var validationResult = new ValidationResult();

            var isValid = ukprn >= 10000000 && ukprn <= 99999999;
            if (isValid)
                validationResult = new ValidationResult();
            else
                validationResult.Errors.Add(new ValidationFailure(nameof(ukprn),
                    _localizer[ResourceMessageName.InvalidUkprn]));

            return validationResult;
        }
    }
}