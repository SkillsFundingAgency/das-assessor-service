using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using OfficeOpenXml.Drawing.Vml;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Helpers;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class
        RegisterAddOrganisationStandardViewModelValidator : AbstractValidator<RegisterAddOrganisationStandardViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;
        private readonly IRegisterValidator _registerValidator;

        public RegisterAddOrganisationStandardViewModelValidator(IOrganisationsApiClient apiClient,
            IRegisterValidator registerValidator)
        {
            _apiClient = apiClient;
            _registerValidator = registerValidator;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult = registerValidator.CheckDateIsEmptyOrValid(vm.EffectiveFromDay,
                    vm.EffectiveFromMonth,
                    vm.EffectiveFromYear, "EffectiveFromDay",
                    "EffectiveFromMonth", "EffectiveFromYear", "EffectiveFrom", "Effective From");


                var validationResultEffectiveTo = registerValidator.CheckDateIsEmptyOrValid(vm.EffectiveToDay,
                    vm.EffectiveToMonth,
                    vm.EffectiveToYear, "EffectiveToDay",
                    "EffectiveToMonth", "EffectiveToYear", "EffectiveTo", "Effective To");

                var validationEffectiveFromComparedToStandards = new ValidationResponse();
                
                var effectiveFrom = ConstructDate(vm.EffectiveFromDay, vm.EffectiveFromMonth, vm.EffectiveFromYear);
                var effectiveTo = ConstructDate(vm.EffectiveToDay, vm.EffectiveToMonth, vm.EffectiveToYear);

                validationEffectiveFromComparedToStandards = registerValidator.CheckOrganisationStandardFromDateIsWithinStandardDateRanges(
                    effectiveFrom, vm.StandardEffectiveFrom, vm.StandardEffectiveTo,
                    vm.StandardLastDateForNewStarts);

                var validationComparingEffectiveFromAndTo =
                    registerValidator.CheckEffectiveFromIsOnOrBeforeEffectiveTo(effectiveFrom, effectiveTo);

                if (validationResult.IsValid && validationResultEffectiveTo.IsValid
                    && validationEffectiveFromComparedToStandards.IsValid && validationComparingEffectiveFromAndTo.IsValid) return;
             
                CreateFailuresInContext(validationResult.Errors, context);
                CreateFailuresInContext(validationResultEffectiveTo.Errors, context);
                CreateFailuresInContext(validationEffectiveFromComparedToStandards.Errors, context);
                CreateFailuresInContext(validationComparingEffectiveFromAndTo.Errors, context);
            });
        }

        private static void CreateFailuresInContext(IEnumerable<ValidationErrorDetail> errs, CustomContext context)
        {
            foreach (var error in errs)
            {
                context.AddFailure(error.Field, error.ErrorMessage);
            }
        }

        private static DateTime? ConstructDate(string dayString, string monthString, string yearString)
        {

            if (!int.TryParse(dayString, out var day) || !int.TryParse(monthString, out var month) ||
                !int.TryParse(yearString, out var year)) return null;

            return new DateTime(year, month, day);
        }
    }
}
