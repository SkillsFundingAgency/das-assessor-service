using System;
using System.Collections.Generic;
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

            if (viewModel.ApplicationDeterminedDate == null)
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail(dateControlName, "Enter a valid application determined date including a day, month and year"));
            }

                //var formatStrings = new string[] { "d/M/yyyy" };
                //if (!DateTime.TryParseExact($"{day}/{month}/{year}", formatStrings, null, DateTimeStyles.None, out _))
                //{
                //    errorMessages.Add(new KeyValuePair<string, string>(question.QuestionId, ValidationDefinition.ErrorMessage));
                //}



                if (viewModel.Day == null)
                {
                    validationResult.Errors.Add(new ValidationErrorDetail("Day", "Day Message"));
                }
            
            

           // viewModel.ErrorMessages = validationResult.Errors;
        
            return validationResult;
        }
    }
}
