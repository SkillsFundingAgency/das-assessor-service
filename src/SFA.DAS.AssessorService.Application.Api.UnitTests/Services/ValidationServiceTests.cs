using System;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services
{

    [TestFixture]
    public class ValidationServiceTests
    {
        private ValidationService _validationService;

        [SetUp]
        public void Setup()
        {
            _validationService = new ValidationService();
        }

        [TestCase("", true)]
        [TestCase("a", false)]
        [TestCase("marky..mark@test.com", false)]
        [TestCase("mark@test.com", true)]
        [TestCase("marky.mark@test.com", true)]
        public void CheckEmailAddress(string emailAddress, bool expectedResult)
        {
            var checkResult = _validationService.CheckEmailIsValid(emailAddress);
            Assert.AreEqual(expectedResult,checkResult);
        }

        [TestCase("", false)]
        [TestCase("a", true)]
        [TestCase(" ", false)]
        public void CheckIsNotEmpty(string stringToCheck, bool expectedResult)
        {
            var checkResult = _validationService.IsNotEmpty(stringToCheck);
            Assert.AreEqual(expectedResult, checkResult);
        }

        [TestCase("1", false)]
        [TestCase("", true)]
        [TestCase(" ", true)]
        [TestCase("10000000", true)]
        [TestCase("99999999", true)]
        [TestCase("100000000", false)]
        [TestCase("999999999", false)]
        [TestCase("1000000", false)]
        [TestCase("9999999", false)]
        public void CheckUkprnIsValid(string stringToCheck, bool expectedResult)
        {
            var checkResult = _validationService.UkprnIsValid(stringToCheck);
            Assert.AreEqual(expectedResult, checkResult);
        }

        [TestCase("", true)]
        [TestCase(" ", true)]
        [TestCase("1", true)]
        [TestCase("1000000", true)]
        [TestCase("9999999", true)]
        [TestCase("9999999999",true)]
        [TestCase("10000000000", false)]

        public void CheckUlnIsValid(string stringToCheck, bool expectedResult)
        {
            var checkResult = _validationService.UlnIsValid(stringToCheck);
            Assert.AreEqual(expectedResult, checkResult);
        }

        [TestCase("", 2, true)]
        [TestCase(" ", 2, true)]
        [TestCase("a", 2, false)]
        [TestCase("a ", 2, false)]
        [TestCase("ab", 2, true)]
        [TestCase("a b", 3, true)]
        [TestCase("a bsdf sdf", 2, true)]

        public void CheckMinimumLength(string stringToCheck, int minimumLength, bool expectedResult)
        {
            var checkResult = _validationService.IsMinimumLengthOrMore(stringToCheck, minimumLength);
            Assert.AreEqual(expectedResult, checkResult);
        }


        [TestCase("", 2, true)]
        [TestCase(" ", 2, true)]
        [TestCase("a", 2, true)]
        [TestCase("a ", 2, true)]
        [TestCase("ab", 1, false)]
        [TestCase("a b", 3, true)]
        [TestCase("a bsdf sdf", 2, false)]
        [TestCase("a bsdf sdf", 10, true)]
        public void CheckMaximumLength(string stringToCheck, int maximumLength, bool expectedResult)
        {
            var checkResult = _validationService.IsMaximumLengthOrLess(stringToCheck, maximumLength);
            Assert.AreEqual(expectedResult, checkResult);
        }

        [TestCase("", true)]
        [TestCase(" ", true)]
        [TestCase(",,", true)]
        [TestCase("1", false)]
        [TestCase("31-DEC-2018", true)]
        [TestCase("2018-01-01", true)]
        [TestCase("1/may/2018", true)]
        [TestCase("28 Feb 2018", true)]
        [TestCase("29 Feb 2018", false)]
        public void CheckDateIsValid(string dateToCheck, bool expectedResult)
        {
            var checkResult = _validationService.DateIsValid(dateToCheck);
            Assert.AreEqual(expectedResult, checkResult);
        }

        [TestCase("", true)]
        [TestCase(" ", true)]
        [TestCase(",,", true)]
        [TestCase("1", true)]
        [TestCase("today", true)]
        [TestCase("yesterday", false)]
        [TestCase("inayear", true)]
        public void CheckDateIsTodayOrInFuture(string dateToCheck, bool expectedResult)
        {
            dateToCheck = MapDateToRelativeWords(dateToCheck);
            var checkResult = _validationService.DateIsTodayOrInFuture(dateToCheck);
            Assert.AreEqual(expectedResult, checkResult);
        }

        [TestCase("", true)]
        [TestCase(" ", true)]
        [TestCase(",,", true)]
        [TestCase("1", true)]
        [TestCase("today", true)]
        [TestCase("yesterday", true)]
        [TestCase("tomorrow", false)]
        [TestCase("inayear", false)]
        public void CheckDateIsTodayOrInPast(string dateToCheck, bool expectedResult)
        {
            dateToCheck = MapDateToRelativeWords(dateToCheck);
            var checkResult = _validationService.DateIsTodayOrInPast(dateToCheck);
            Assert.AreEqual(expectedResult, checkResult);
        }

        [TestCase("", true)]
        [TestCase(" ", true)]
        [TestCase("EPA1", false)]
        [TestCase("EPA22", false)]
        [TestCase("EPA333", false)]
        [TestCase("EPA4444", true)]
        [TestCase("EPa55555", true)]
        [TestCase("EpA666666", true)]
        [TestCase("ePA7777777", true)]
        [TestCase("epA88888888", true)]
        [TestCase("Epa999999999", true)]
        [TestCase("EPA33333A", false)]
        [TestCase("EPAA33333", false)]
        public void CheckOrganisationId(string organisationIdToCheck, bool expectedResult)
        {
            var checkResult = _validationService.OrganisationIdIsValid(organisationIdToCheck);
            Assert.AreEqual(expectedResult, checkResult);
        }

        [TestCase("33333", false)]
        [TestCase("3333333", false)]
        [TestCase("33333333", true)]
        [TestCase("rc333333", true)]
        public void CheckCompanyNumber(string companyNumberToCheck, bool expectedResult)
        {
            var checkResult = _validationService.CompanyNumberIsValid(companyNumberToCheck);
            Assert.AreEqual(expectedResult, checkResult);
        }

        [TestCase("33333", true)]
        [TestCase("3333333", true)]
        [TestCase("33333333", true)]
        [TestCase("RC333333", false)]
        [TestCase("3333333-3", true)]
        [TestCase("3-3333333", true)]
        public void CheckCharityNumber(string charityNumberToCheck, bool expectedResult)
        {
            var checkResult = _validationService.CharityNumberIsValid(charityNumberToCheck);
            Assert.AreEqual(expectedResult, checkResult);
        }

        private string MapDateToRelativeWords(string dateCheck)
        {
            switch (dateCheck)
            {
                case "today":
                    return DateTime.Today.ToString("dd-MMM-yyyy");
                case "yesterday":
                    return DateTime.Today.AddDays(-1).ToString("dd-MMM-yyyy");
                case "tomorrow":
                    return DateTime.Today.AddDays(1).ToString("dd-MMM-yyyy");
                case "inayear":
                    return DateTime.Today.AddYears(1).ToString("dd-MMM-yyyy");
                default:
                    return dateCheck;
            }
        }
    }
}