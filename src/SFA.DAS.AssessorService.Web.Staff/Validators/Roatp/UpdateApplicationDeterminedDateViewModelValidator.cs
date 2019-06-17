using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Web.Staff.Resources;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public class UpdateApplicationDeterminedDateViewModelValidator : AbstractValidator<UpdateApplicationDeterminedDateViewModel>
    {
        public UpdateApplicationDeterminedDateViewModelValidator()
        {
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

        private ValidationResponse IsaValidDeterminedDate(UpdateApplicationDeterminedDateViewModel viewModel)
        {
            var dateControlName = "ApplicationDeterminedDate";

            var validationResult = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (viewModel.Day == null && viewModel.Month == null && viewModel.Year == null)
            {
                validationResult.Errors.Add(new ValidationErrorDetail(dateControlName, RoatpOrganisationValidation.ApplicationDeterminedDateNoFieldsEntered));
                validationResult.Errors.Add(new ValidationErrorDetail("Day", "day error"));
                validationResult.Errors.Add(new ValidationErrorDetail("Month", "month error"));
                validationResult.Errors.Add(new ValidationErrorDetail("Year", "year error"));
                return validationResult;
            }


            if (string.IsNullOrWhiteSpace(viewModel?.Day.ToString()) ||
                string.IsNullOrWhiteSpace(viewModel?.Month.ToString()) ||
                string.IsNullOrWhiteSpace(viewModel?.Year.ToString()))
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail(dateControlName, RoatpOrganisationValidation.ApplicationDeterminedDateFieldsNotEntered));

                if (viewModel?.Day == null)
                    validationResult.Errors.Add(new ValidationErrorDetail("Day", "day error"));
                if (viewModel?.Month == null)
                    validationResult.Errors.Add(new ValidationErrorDetail("Month", "month error"));
                if (viewModel?.Year == null)
                    validationResult.Errors.Add(new ValidationErrorDetail("Year", "year error"));
                return validationResult;
            }

            var formatStrings = new string[] { "d/M/yyyy" };
            var yearWithCentury = viewModel?.Year;
            if (yearWithCentury != null && yearWithCentury < 99)
                yearWithCentury += 2000;

            if (!DateTime.TryParseExact($"{viewModel?.Day}/{viewModel?.Month}/{yearWithCentury}", formatStrings, null, DateTimeStyles.None, out _))
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail(dateControlName, RoatpOrganisationValidation.ApplicationDeterminedDateInvalidDates));
                return validationResult;
            }

            if (viewModel.ApplicationDeterminedDate > DateTime.Today)
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail(dateControlName, RoatpOrganisationValidation.ApplicationDeterminedDateFutureDate));
            }

            return validationResult;
        }
    }
}