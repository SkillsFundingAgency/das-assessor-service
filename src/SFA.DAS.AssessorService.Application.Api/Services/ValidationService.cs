using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Text.RegularExpressions;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class ValidationService : IValidationService
    {
        public bool CharityNumberIsValid(string charityNumberToCheck)
        {
            if (string.IsNullOrEmpty(charityNumberToCheck?.Trim())) return true;
            var regex = new Regex(@"[0-9-]{1,}");
            return regex.Match(charityNumberToCheck).Success && regex.Match(charityNumberToCheck).Value == charityNumberToCheck;
        }

        public bool CheckEmailIsValid(string emailToCheck)
        {
            if (string.IsNullOrEmpty(emailToCheck?.Trim())) return true;
            var validationResults = new EmailCheckValidator().Validate(new EmailCheck { EmailToCheck = emailToCheck });
            return validationResults.IsValid;
        }

        public bool CompanyNumberIsValid(string companyNumberToCheck)
        {
            if (string.IsNullOrEmpty(companyNumberToCheck?.Trim())) return true;
            var regex = new Regex(@"[A-Za-z0-9]{2}[0-9]{6,}");
            return regex.Match(companyNumberToCheck).Success;
        }

        public bool DateIsTodayOrInFuture(string dateToCheck)
        {
            if (string.IsNullOrEmpty(dateToCheck?.Trim()) || dateToCheck?.Trim() == ",,") return true;
            if (!DateTime.TryParse(dateToCheck, out DateTime dateProcessed)) return true;
            return dateProcessed >= DateTime.Today;
        }

        public bool DateIsTodayOrInPast(string dateToCheck)
        {
            if (string.IsNullOrEmpty(dateToCheck?.Trim()) || dateToCheck?.Trim() == ",,") return true;
            if (!DateTime.TryParse(dateToCheck, out DateTime dateProcessed)) return true;
            return dateProcessed <= DateTime.Today;
        }

        public bool DateIsValid(string dateToCheck)
        {
            if (string.IsNullOrEmpty(dateToCheck?.Trim()) || dateToCheck?.Trim()==",,") return true;
            return DateTime.TryParse(dateToCheck, out DateTime _);
        }

        public bool IsMaximumLengthOrLess(string stringToCheck, int maximumLength)
        {
            if (string.IsNullOrEmpty(stringToCheck?.Trim())) return true;
            return stringToCheck.Trim().Length <= maximumLength;
        }

        public bool IsMinimumLengthOrMore(string stringToCheck, int minimumLength)
        {
            if (string.IsNullOrEmpty(stringToCheck?.Trim())) return true;
            return stringToCheck.Trim().Length >= minimumLength;
        }

        public bool IsNotEmpty(string stringToCheck)
        {
            return !string.IsNullOrEmpty(stringToCheck?.Trim());
        }

        public bool OrganisationIdIsValid(string organisationIdToCheck)
        {
            if (string.IsNullOrEmpty(organisationIdToCheck?.Trim())) return true;
            var regex = new Regex(@"[eE][pP][aA][0-9]{4,9}$");
                return regex.Match(organisationIdToCheck).Success;
        }

        public bool UkprnIsValid(string ukprnToCheck)
        {
            if (string.IsNullOrEmpty(ukprnToCheck?.Trim())) return true;
            if (!int.TryParse(ukprnToCheck, out int ukprn))
                return false;

            return ukprn >= 10000000 && ukprn <= 99999999;
        }

        public bool UlnIsValid(string ulnToCheck)
        {
            if (string.IsNullOrEmpty(ulnToCheck?.Trim())) return true;
            if (!long.TryParse(ulnToCheck, out long uln))
                return false;

            return uln  <= 9999999999 && uln>=0;
        }
    }
}
