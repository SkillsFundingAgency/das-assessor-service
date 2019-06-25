using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using System;
using System.Collections.Generic;
using System.Globalization;
using SFA.DAS.AssessorService.Web.Staff.Resources;

namespace SFA.DAS.AssessorService.Web.Staff.Services.Roatp
{
    public class ApplicationDeterminedDateValidationService: IApplicationDeterminedDateValidationService
    {
        public ValidationResponse ValidateApplicationDeterminedDate(int? day, int? month, int? year)
        {
            var dateControlName = "ApplicationDeterminedDate";

            var validationResult = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (day == null && month == null && year == null)
            {
                validationResult.Errors.Add(new ValidationErrorDetail(dateControlName, RoatpOrganisationValidation.ApplicationDeterminedDateNoFieldsEntered));
                validationResult.Errors.Add(new ValidationErrorDetail("Day", RoatpOrganisationValidation.ApplicationDeterminedDateFutureDay));
                validationResult.Errors.Add(new ValidationErrorDetail("Month", RoatpOrganisationValidation.ApplicationDeterminedDateFutureMonth));
                validationResult.Errors.Add(new ValidationErrorDetail("Year", RoatpOrganisationValidation.ApplicationDeterminedDateFutureYear));
                return validationResult;
            }


            if (string.IsNullOrWhiteSpace(day.ToString()) ||
                string.IsNullOrWhiteSpace(month.ToString()) ||
                string.IsNullOrWhiteSpace(year.ToString()))
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail(dateControlName, RoatpOrganisationValidation.ApplicationDeterminedDateFieldsNotEntered));

                if (day == null)
                    validationResult.Errors.Add(new ValidationErrorDetail("Day", RoatpOrganisationValidation.ApplicationDeterminedDateFutureDay));
                if (month == null)
                    validationResult.Errors.Add(new ValidationErrorDetail("Month", RoatpOrganisationValidation.ApplicationDeterminedDateFutureMonth));
                if (year == null)
                    validationResult.Errors.Add(new ValidationErrorDetail("Year", RoatpOrganisationValidation.ApplicationDeterminedDateFutureYear));
                return validationResult;
            }

            var formatStrings = new string[] { "d/M/yyyy" };
            var yearWithCentury = year;
            if (yearWithCentury != null && yearWithCentury <= 99)
                yearWithCentury += 2000;

            if (!DateTime.TryParseExact($"{day}/{month}/{yearWithCentury}", formatStrings, null, DateTimeStyles.None, out DateTime applicationDeterminedDate))
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail(dateControlName, RoatpOrganisationValidation.ApplicationDeterminedDateInvalidDates));
                return validationResult;
            }

            if (applicationDeterminedDate > DateTime.Today)
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail(dateControlName, RoatpOrganisationValidation.ApplicationDeterminedDateFutureDate));
            }

            return validationResult;
        }
    }

    
}
