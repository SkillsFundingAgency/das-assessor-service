using System;
using Castle.Core.Internal;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Register.OrganisationStandards
{
    [TestFixture]
    public class EpaOrganisationStandardsValidatorEffectiveFromFallsBeforeEffectivetoTests
    {
        private Mock<IStringLocalizer<EpaOrganisationValidator>> _localizer;
        private EpaOrganisationValidator _validator;

        [SetUp]
        public void Setup()
        {
            _localizer = new Mock<IStringLocalizer<EpaOrganisationValidator>>();
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveFromAfterEffectiveTo])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveFromAfterEffectiveTo, "fail"));
        
            _validator = new EpaOrganisationValidator(Mock.Of<IRegisterValidationRepository>(), Mock.Of<IRegisterQueryRepository>(), Mock.Of<ISpecialCharacterCleanserService>(), _localizer.Object);

        }


        [Test]
        public void RegisterValidationOfOrganisationStandardEffectiveFromAgainstStandardDetailsValueSource(
            [ValueSource(nameof(LocalTestData))]
            TestData testData)
        {
            var results = _validator.CheckEffectiveFromIsOnOrBeforeEffectiveTo(
                testData.EffectiveFrom,
                testData.EffectiveTo);

            Assert.AreEqual(results.Length==0, testData.IsValid);
        }


        public class TestData
        {
            public DateTime? EffectiveFrom { get; set; }
            public DateTime? EffectiveTo { get; set; }
            public bool IsValid { get; set; }
        }


        public static readonly TestData[] LocalTestData =
        {
            new TestData {EffectiveFrom = null, EffectiveTo = null, IsValid = true},
            new TestData {EffectiveFrom = DateTime.Today, EffectiveTo = null, IsValid = true},
            new TestData {EffectiveFrom = null, EffectiveTo = DateTime.Today, IsValid = true},
            new TestData {EffectiveFrom = DateTime.Today, EffectiveTo = DateTime.Today, IsValid = true},
            new TestData {EffectiveFrom = DateTime.Today.AddDays(-1), EffectiveTo = DateTime.Today, IsValid = true},
            new TestData {EffectiveFrom = DateTime.Today.AddDays(1), EffectiveTo = DateTime.Today, IsValid = false},
            new TestData {EffectiveFrom = DateTime.Today, EffectiveTo = DateTime.Today.AddDays(1), IsValid = true}
        };


    }
}
