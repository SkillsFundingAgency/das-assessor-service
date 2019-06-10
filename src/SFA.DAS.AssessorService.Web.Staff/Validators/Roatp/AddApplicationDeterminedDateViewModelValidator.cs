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
    public class AddApplicationDeterminedDateViewModelValidator : AbstractValidator<AddApplicationDeterminedDateViewModel>
    {
        public AddApplicationDeterminedDateViewModelValidator()
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

        private ValidationResponse IsaValidDeterminedDate(AddApplicationDeterminedDateViewModel viewModel)
        {
            var dateControlName = "ApplicationDeterminedDate";

            var validationResult = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (viewModel.Day == null && viewModel.Month == null && viewModel.Year == null)
            {
                validationResult.Errors.Add(new ValidationErrorDetail(dateControlName, "Enter the application determined date"));
                return validationResult;
            }


            if (string.IsNullOrWhiteSpace(viewModel?.Day.ToString()) || 
                    string.IsNullOrWhiteSpace(viewModel?.Month.ToString()) || 
                    string.IsNullOrWhiteSpace(viewModel?.Year.ToString()))
                {
                    validationResult.Errors.Add(
                        new ValidationErrorDetail(dateControlName, "Enter the application determined date including a day, month and year"));
                }

            var formatStrings = new string[] { "d/M/yyyy" };
            var yearWithCentury = viewModel?.Year;
            if (yearWithCentury!=null && yearWithCentury < 99)
                yearWithCentury += 2000;

            if (!DateTime.TryParseExact($"{viewModel?.Day}/{viewModel?.Month}/{yearWithCentury}", formatStrings, null, DateTimeStyles.None, out DateTime formattedDate))
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail(dateControlName, "Enter a valid application determined date including a day, month and year"));
            }

            if (viewModel.ApplicationDeterminedDate > DateTime.Today)
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail(dateControlName, "Application determined date must be today or in the past"));
            }
        
            return validationResult;
        }
    }
}
