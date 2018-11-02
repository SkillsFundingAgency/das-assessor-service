using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Staff.Helpers;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class RegisterEditOrganisationStandardViewModel : AbstractValidator<RegisterViewAndEditOrganisationStandardViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;
        private readonly IRegisterValidator _registerValidator;
        public RegisterEditOrganisationStandardViewModel(IOrganisationsApiClient apiClient,
            IRegisterValidator registerValidator)
        {
            _apiClient = apiClient;
            _registerValidator = registerValidator;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResultEffectiveFrom = registerValidator.CheckDateIsEmptyOrValid(vm.EffectiveFromDay,
                    vm.EffectiveFromMonth,
                    vm.EffectiveFromYear, "EffectiveFromDay",
                    "EffectiveFromMonth", "EffectiveFromYear", "EffectiveFrom", "Effective From");


                var validationResultEffectiveTo = registerValidator.CheckDateIsEmptyOrValid(vm.EffectiveToDay,
                    vm.EffectiveToMonth,
                    vm.EffectiveToYear, "EffectiveToDay",
                    "EffectiveToMonth", "EffectiveToYear", "EffectiveTo", "Effective To");

                vm.EffectiveFrom = ConstructDate(vm.EffectiveFromDay, vm.EffectiveFromMonth, vm.EffectiveFromYear);
                vm.EffectiveTo = ConstructDate(vm.EffectiveToDay, vm.EffectiveToMonth, vm.EffectiveToYear);

                CreateFailuresInContext(validationResultEffectiveFrom.Errors, context);
                CreateFailuresInContext(validationResultEffectiveTo.Errors, context);

                var deliveryAreas = vm.DeliveryAreas ?? new List<int>();
                var validationResultExternals = _apiClient
                    .ValidateUpdateOrganisationStandard(vm.OrganisationId, vm.StandardId, vm.EffectiveFrom,
                        vm.EffectiveTo, vm.ContactId, deliveryAreas, vm.ActionChoice).Result;
                if (validationResultExternals.IsValid) return;
                foreach (var error in validationResultExternals.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }

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
