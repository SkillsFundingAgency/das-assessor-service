using System;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Staff.Helpers;


namespace SFA.DAS.AssessorService.Web.Staff.Tests.Helpers
{
    [TestFixture]
    public class RegisterValidatorCheckOrganisationStandardEffectiveFromValidationTests
    {
        private readonly RegisterValidator _validator = new RegisterValidator(); 

        [Test]
        public void RegisterValidationOfOrganisationStandardEffectiveFromAgainstStandardDetails(
            [ValueSource(nameof(_testDateForEffectiveFrom))]
            TestDataForEffectiveFrom testData)
        {
            var results = _validator.CheckOrganisationStandardFromDateIsWithinStandardDateRanges(
                testData.OrgStandardEffectiveFrom,
                testData.StandardEffectiveFrom,
                testData.StandardEffectiveTo,
                testData.StandardLastDateForNewStarts);
            
            Assert.AreEqual(results.IsValid, testData.IsValid);
        }

        public class TestDataForEffectiveFrom
        {
            public DateTime? OrgStandardEffectiveFrom { get; set; }
            public DateTime StandardEffectiveFrom { get; set; }
            public DateTime? StandardEffectiveTo { get; set; }
            public DateTime? StandardLastDateForNewStarts { get; set; }
            public bool IsValid { get; set; }
        }

        private static readonly TestDataForEffectiveFrom[] _testDateForEffectiveFrom =
        {
            new TestDataForEffectiveFrom
            {
                OrgStandardEffectiveFrom = null,
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = null,
                StandardLastDateForNewStarts = null,
                IsValid = true
            },
            new TestDataForEffectiveFrom
            {
                OrgStandardEffectiveFrom = DateTime.Today,
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = null,
                StandardLastDateForNewStarts = null,
                IsValid = true
            },new TestDataForEffectiveFrom
            {
                OrgStandardEffectiveFrom = DateTime.Today.AddDays(-1),
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = null,
                StandardLastDateForNewStarts = null,
                IsValid = false
            },new TestDataForEffectiveFrom
            {
                OrgStandardEffectiveFrom = DateTime.Today.AddDays(10),
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = DateTime.Today.AddDays(10),
                StandardLastDateForNewStarts = null,
                IsValid = true
            },new TestDataForEffectiveFrom
            {
                OrgStandardEffectiveFrom = DateTime.Today.AddDays(11),
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = DateTime.Today.AddDays(10),
                StandardLastDateForNewStarts = null,
                IsValid = false
            },new TestDataForEffectiveFrom
            {
                OrgStandardEffectiveFrom = DateTime.Today.AddDays(10),
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = DateTime.Today.AddDays(10),
                StandardLastDateForNewStarts = DateTime.Today.AddDays(-9),
                IsValid = false
            }
            
        };
    }
}