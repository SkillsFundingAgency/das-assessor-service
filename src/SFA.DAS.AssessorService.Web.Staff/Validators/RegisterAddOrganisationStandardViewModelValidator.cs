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

               
             
                CreateFailuresInContext(validationResult.Errors, context);
                CreateFailuresInContext(validationResultEffectiveTo.Errors, context);

                if (validationResult.IsValid && validationResultEffectiveTo.IsValid)
                {
                    var validationResultExternals = _apiClient
                        .ValidateCreateOrganisationStandard(vm.OrganisationId, vm.StandardId, vm.EffectiveFrom,
                            vm.EffectiveTo, vm.ContactId, vm.DeliveryAreas).Result;
                    if (validationResultExternals.IsValid) return;
                    foreach (var error in validationResultExternals.Errors)
                    {
                        context.AddFailure(error.Field, error.ErrorMessage);
                    }
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

       
    }
}
