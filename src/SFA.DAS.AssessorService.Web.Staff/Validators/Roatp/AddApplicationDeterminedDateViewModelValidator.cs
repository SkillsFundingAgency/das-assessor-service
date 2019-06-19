using System;
using System.Collections.Generic;
using System.Globalization;
using FluentValidation;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Web.Staff.Resources;
using SFA.DAS.AssessorService.Web.Staff.Services.Roatp;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public class
        AddApplicationDeterminedDateViewModelValidator : AbstractValidator<AddApplicationDeterminedDateViewModel>
    {
        private readonly IApplicationDeterminedDateValidationService _applicationDeterminedDateValidationService;

        public AddApplicationDeterminedDateViewModelValidator(
            IApplicationDeterminedDateValidationService applicationDeterminedDateValidationService)
        {
            _applicationDeterminedDateValidationService = applicationDeterminedDateValidationService;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult = IsaValidDeterminedDate(vm);
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }

        private ValidationResponse IsaValidDeterminedDate(AddApplicationDeterminedDateViewModel viewModel)
        {
            return _applicationDeterminedDateValidationService.ValidateApplicationDeterminedDate(viewModel.Day,
                viewModel.Month, viewModel.Year);

            //    var dateControlName = "ApplicationDeterminedDate";

            //    var validationResult = new ValidationResponse
            //    {
            //        Errors = new List<ValidationErrorDetail>()
            //    };

            //    if (viewModel.Day == null && viewModel.Month == null && viewModel.Year == null)
            //    {
            //        validationResult.Errors.Add(new ValidationErrorDetail(dateControlName, RoatpOrganisationValidation.ApplicationDeterminedDateNoFieldsEntered));
            //        validationResult.Errors.Add(new ValidationErrorDetail("Day", RoatpOrganisationValidation.ApplicationDeterminedDateFutureDay));
            //        validationResult.Errors.Add(new ValidationErrorDetail("Month", RoatpOrganisationValidation.ApplicationDeterminedDateFutureMonth));
            //        validationResult.Errors.Add(new ValidationErrorDetail("Year", RoatpOrganisationValidation.ApplicationDeterminedDateFutureYear));
            //        return validationResult;
            //    }


            //    if (string.IsNullOrWhiteSpace(viewModel?.Day.ToString()) || 
            //            string.IsNullOrWhiteSpace(viewModel?.Month.ToString()) || 
            //            string.IsNullOrWhiteSpace(viewModel?.Year.ToString()))
            //        {
            //            validationResult.Errors.Add(
            //                new ValidationErrorDetail(dateControlName, RoatpOrganisationValidation.ApplicationDeterminedDateFieldsNotEntered));

            //        if (viewModel?.Day == null)
            //            validationResult.Errors.Add(new ValidationErrorDetail("Day", RoatpOrganisationValidation.ApplicationDeterminedDateFutureDay));
            //        if (viewModel?.Month == null)
            //            validationResult.Errors.Add(new ValidationErrorDetail("Month", RoatpOrganisationValidation.ApplicationDeterminedDateFutureMonth));
            //        if (viewModel?.Year == null)
            //            validationResult.Errors.Add(new ValidationErrorDetail("Year", RoatpOrganisationValidation.ApplicationDeterminedDateFutureYear));
            //        return validationResult;
            //    }

            //    var formatStrings = new string[] { "d/M/yyyy" };
            //    var yearWithCentury = viewModel?.Year;
            //    if (yearWithCentury!=null && yearWithCentury <= 99)
            //        yearWithCentury += 2000;

            //    if (!DateTime.TryParseExact($"{viewModel?.Day}/{viewModel?.Month}/{yearWithCentury}", formatStrings, null, DateTimeStyles.None, out _))
            //    {
            //        validationResult.Errors.Add(
            //            new ValidationErrorDetail(dateControlName, RoatpOrganisationValidation.ApplicationDeterminedDateInvalidDates));
            //        return validationResult;
            //    }

            //    if (viewModel.ApplicationDeterminedDate > DateTime.Today)
            //    {
            //        validationResult.Errors.Add(
            //            new ValidationErrorDetail(dateControlName, RoatpOrganisationValidation.ApplicationDeterminedDateFutureDate));
            //    }

            //    return validationResult;
            //}
        }
    }
}
