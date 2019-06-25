using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Resources;
using SFA.DAS.AssessorService.Web.Staff.Services.Roatp;
using SFA.DAS.AssessorService.Web.Staff.Validators.Roatp;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Validators
{
    [TestFixture]
    public class AddApplicationDeterminedDateViewModelValidatorTests
    {
        private AddApplicationDeterminedDateViewModel _viewModel;

        [SetUp]
        public void Before_each_test()
        {
        }

        [TestCase(2019)]
        [TestCase(19)]
        public void Validator_passes_valid_date(int year)
        {   
            _viewModel = new AddApplicationDeterminedDateViewModel { Day = 10, Month = 4, Year=year };

            var validator = new AddApplicationDeterminedDateViewModelValidator(new ApplicationDeterminedDateValidationService());
            var validationResult = validator.Validate(_viewModel);

            Assert.AreEqual(0, validationResult.Errors.Count);
        }

        [TestCase(null,null,null,4,"noDetails")]
        [TestCase(10, null, null,3, "partialDetails")]
        [TestCase(null, 4, null,3, "partialDetails")]
        [TestCase(null, null, 2019,3, "partialDetails")]
        [TestCase(10, 4, null, 2, "partialDetails")]
        [TestCase(10, null, 2019, 2, "partialDetails")]
        [TestCase(null, 4, 2019, 2, "partialDetails")]
        [TestCase(31, 2, 2019, 1, "invalidDateDetails")]
        [TestCase(31, 2, 19, 1, "invalidDateDetails")]
        [TestCase(1, 1, 2019, 1, "futureDateDetails")]
        public void Validator_fails_when_invalid_details_added(int? day, int? month, int? year, int  numberOfErrors, string errorMessageType)
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

            _viewModel = new AddApplicationDeterminedDateViewModel { Day = day, Month = month, Year = year };

            var validator = new AddApplicationDeterminedDateViewModelValidator(new ApplicationDeterminedDateValidationService());
            var validationResult = validator.Validate(_viewModel);

            Assert.AreEqual(numberOfErrors, validationResult.Errors.Count);
            Assert.IsTrue(validationResult.Errors.Any(x=> x.ErrorMessage==errorMessage));
        }
    }
}

