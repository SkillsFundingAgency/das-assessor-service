using System;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.AssessorService.Web.Staff.Helpers;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Helpers
{
    [TestFixture]
    public class RegisterValidationCheckEffectiveFromFallsOnOrBeforeEffectiveToTests
    {
        private readonly RegisterValidator _validator = new RegisterValidator();

        [Test]
        public void RegisterValidationOfOrganisationStandardEffectiveFromAgainstStandardDetailsValueSource( 
            [ValueSource(nameof(_testData))]
            TestData testData)
        {
            var results = _validator.CheckEffectiveFromIsOnOrBeforeEffectiveTo(
                testData.EffectiveFrom,
                testData.EffectiveTo);
            
            Assert.AreEqual(results.IsValid, testData.IsValid); 
        }


        public class TestData
        {
            public DateTime? EffectiveFrom { get; set; }
            public DateTime? EffectiveTo { get; set; }
            public bool IsValid { get; set; }
        }


        public static readonly TestData[] _testData =
        {
            new TestData {EffectiveFrom = null, EffectiveTo = null, IsValid = true},
            new TestData {EffectiveFrom = DateTime.Today, EffectiveTo = null, IsValid = true},
            new TestData {EffectiveFrom = null, EffectiveTo = DateTime.Today, IsValid = true},
            new TestData {EffectiveFrom = DateTime.Today, EffectiveTo = DateTime.Today, IsValid = true},
            new TestData {EffectiveFrom = DateTime.Today.AddDays(-1), EffectiveTo = DateTime.Today, IsValid = true},
            new TestData {EffectiveFrom = DateTime.Today.AddDays(1), EffectiveTo = DateTime.Today, IsValid = false}
        };
    }
}