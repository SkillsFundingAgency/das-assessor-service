using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Staff.Resources;
using SFA.DAS.AssessorService.Web.Staff.Services.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Services
{
    [TestFixture]
    public class ApplicationDeterminedDateValidationServiceTests
    {
        [TestCase(null, null, null, 4, "noDetails")]
        [TestCase(10, null, null, 3, "partialDetails")]
        [TestCase(null, 4, null, 3, "partialDetails")]
        [TestCase(null, null, 2019, 3, "partialDetails")]
        [TestCase(10, 4, null, 2, "partialDetails")]
        [TestCase(10, null, 2019, 2, "partialDetails")]
        [TestCase(null, 4, 2019, 2, "partialDetails")]
        [TestCase(31, 2, 2019, 1, "invalidDateDetails")]
        [TestCase(31, 2, 19, 1, "invalidDateDetails")]
        [TestCase(1, 1, 2019, 1, "futureDateDetails")]
        public void ValidatorService_returns_expected_responses_when_invalid_details_added(int? day, int? month, int? year, int numberOfErrors,
            string errorMessageType)
        {
            var errorMessage = string.Empty;
            switch (errorMessageType)
            {
                case "noDetails":
                    errorMessage = RoatpOrganisationValidation.ApplicationDeterminedDateNoFieldsEntered;
                    break;
                case "partialDetails":
                    errorMessage = RoatpOrganisationValidation.ApplicationDeterminedDateFieldsNotEntered;
                    break;
                case "invalidDateDetails":
                    errorMessage = RoatpOrganisationValidation.ApplicationDeterminedDateInvalidDates;
                    break;
                case "futureDateDetails":
                    errorMessage = RoatpOrganisationValidation.ApplicationDeterminedDateFutureDate;
                    year = DateTime.Today.Year + 1;
                    break;
            }


            var validationResult =
                new ApplicationDeterminedDateValidationService().ValidateApplicationDeterminedDate(day, month, year);

            Assert.AreEqual(numberOfErrors, validationResult.Errors.Count);
            Assert.IsTrue(validationResult.Errors.Any(x => x.ErrorMessage == errorMessage));
        }
    }
}