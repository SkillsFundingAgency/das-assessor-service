using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;


namespace SFA.DAS.AssessorService.Web.Staff.Helpers
{
    public class RegisterValidator : IRegisterValidator
    {
        public ValidationResponse CheckDateIsEmptyOrValid(string day, string month, string year, 
                                                            string dayFieldName, string monthFieldName, string yearFieldName, 
                                                            string dateFieldName, string dateFieldDescription)
        {
            var validationResponse = new ValidationResponse();
            if (string.IsNullOrEmpty(day) && string.IsNullOrEmpty(month) && string.IsNullOrEmpty(year))
            {
                return validationResponse;
            }

            ValidateDay(day, dayFieldName, dateFieldDescription, validationResponse);
            ValidateMonth(month, monthFieldName, dateFieldDescription, validationResponse);
            ValidateYear(year, yearFieldName, dateFieldDescription, validationResponse);
            if (validationResponse.IsValid)
                ValidateDate(day, month, year, dateFieldName, dateFieldDescription, validationResponse);
            return validationResponse;
        }

        private void ValidateDate(string dayString, string monthString, string yearString, string dateField, string description, ValidationResponse validationResponse)
        {
            if (!int.TryParse(dayString, out var day) || !int.TryParse(monthString, out var month) ||
                !int.TryParse(yearString, out var year)) return;
            try
            {
               var checkedDate= new DateTime(year, month, day);
                if (checkedDate> DateTime.Now.AddYears(20) || checkedDate < DateTime.Now.AddYears(-20))
                {
                    RunValidationCheckAndAppendAnyError(dateField, $"Enter a valid date for {description}", validationResponse);
                }
            }
            catch
            {
                RunValidationCheckAndAppendAnyError(dateField, $"Enter a valid date for {description}", validationResponse);
            }
            
        }

        private void ValidateYear(string year, string yearFieldName, string description, ValidationResponse validationResponse)
        {
            if (string.IsNullOrEmpty(year))
            {
                RunValidationCheckAndAppendAnyError(yearFieldName, $"Enter the year for {description}", validationResponse);
            }
            else
            {
                if (!BeANumber(year))
                {
                    RunValidationCheckAndAppendAnyError(yearFieldName, $"Year must be a number for {description}", validationResponse);
                }
                else
                {
                    if (year.Length<4)
                        RunValidationCheckAndAppendAnyError(yearFieldName, $"Enter a valid year for {description}", validationResponse);
                }
            }
        }

        private void ValidateMonth(string month, string monthFieldName, string description, ValidationResponse validationResponse)
        {
            if (string.IsNullOrEmpty(month))
            {
                RunValidationCheckAndAppendAnyError(monthFieldName, $"Enter the month for {description}", validationResponse);
            }
            else
            {
                if (!BeANumber(month))
                {
                    RunValidationCheckAndAppendAnyError(monthFieldName, $"Month must be a number for {description}", validationResponse);
                }
                else
                {
                    if (!BeInValidMonthRange(month))
                        RunValidationCheckAndAppendAnyError(monthFieldName, $"Enter a valid month for {description}", validationResponse);
                }
            }
        }

        private void ValidateDay(string day, string dateFieldName, string description, ValidationResponse validationResponse)
        {
            if (string.IsNullOrEmpty(day))
            {
                RunValidationCheckAndAppendAnyError(dateFieldName, $"Enter the day for {description}", validationResponse);
            }
            else
            {
                if (!BeANumber(day))
                {
                    RunValidationCheckAndAppendAnyError(dateFieldName, $"Day must be a number for {description}", validationResponse);
                }
                else
                {
                    if (!BeInValidDayRange(day))
                        RunValidationCheckAndAppendAnyError(dateFieldName, $"Enter a valid day for {description}", validationResponse);
                }
            }
        }

        private bool BeInValidMonthRange(string monthString)
        {
            var month = int.Parse(monthString);
            return month >= 1 && month <= 12;
        }

        private bool BeInValidDayRange(string dayString)
        {
            var day = int.Parse(dayString);
            return day >= 1 && day <= 31;
        }


        private bool BeANumber(string datePart)
        {
            return int.TryParse(datePart, out int _);
        }
        private void RunValidationCheckAndAppendAnyError(string fieldName, string errorMessage, ValidationResponse validationResult)
        {
            if (errorMessage != string.Empty)
                validationResult.Errors.Add(new ValidationErrorDetail(fieldName, errorMessage.Replace("; ", ""), ValidationStatusCode.BadRequest));
        }
    }
}