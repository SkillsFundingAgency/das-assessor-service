using SFA.DAS.AssessorService.Application.Interfaces;
using System;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class ValidationService : IValidationService
    {
        public bool CheckEmailIsValid(string emailToCheck)
        {
            if (string.IsNullOrEmpty(emailToCheck)) return true;
            var validationResults = new EmailCheckValidator().Validate(new EmailCheck { EmailToCheck = emailToCheck });
            return validationResults.IsValid;
        }

        public bool IsNotEmpty(string stringToCheck)
        {
            return !string.IsNullOrEmpty(stringToCheck);
        }

        public bool UkprnIsValid(string ukprnToCheck)
        {
            if (string.IsNullOrEmpty(ukprnToCheck)) return true;
            if (!int.TryParse(ukprnToCheck, out int ukprn))
                return false;

            return ukprn >= 10000000 && ukprn <= 99999999;
        }
    }
}
