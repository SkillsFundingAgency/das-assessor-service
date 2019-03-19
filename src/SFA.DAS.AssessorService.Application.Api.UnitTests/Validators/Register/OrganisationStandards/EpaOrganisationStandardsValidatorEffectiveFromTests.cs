using System;
using Castle.Core.Internal;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.Services;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Register.OrganisationStandards
{

    [TestFixture]
    public class EpaOrganisationStandardsValidatorEffectiveFromTests
    {
        private EpaOrganisationValidator _validator;
        private Mock<IStringLocalizer<EpaOrganisationValidator>> _localizer;
        private Mock<IStandardService> _standardService;

        [SetUp]
        public void Setup()
        {
            _localizer = new Mock<IStringLocalizer<EpaOrganisationValidator>>();
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveFromBeforeStandardEffectiveFrom])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveFromBeforeStandardEffectiveFrom, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveFromAfterStandardEffectiveTo])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveFromAfterStandardEffectiveTo, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveFromAfterStandardLastDayForNewStarts])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveFromAfterStandardLastDayForNewStarts, "fail"));

            _standardService = new Mock<IStandardService>();
            _validator = new EpaOrganisationValidator(Mock.Of<IRegisterValidationRepository>(),Mock.Of<IRegisterQueryRepository>(),Mock.Of<ISpecialCharacterCleanserService>(),_localizer.Object, _standardService.Object);
        }

        [Test]
        public void RegisterValidationOfOrganisationStandardEffectiveFromAgainstStandardDetails(
            [ValueSource(nameof(TestDateForEffectiveFrom))] TestDataForEffectiveFrom testData)
        {
            var results = _validator.CheckOrganisationStandardFromDateIsWithinStandardDateRanges(
                testData.OrgStandardEffectiveFrom,
                testData.StandardEffectiveFrom,
                testData.StandardEffectiveTo,
                testData.StandardLastDateForNewStarts);

            Assert.AreEqual(results.IsNullOrEmpty(), testData.IsValid);
        }

        public class TestDataForEffectiveFrom
        {
            public DateTime? OrgStandardEffectiveFrom { get; set; }
            public DateTime StandardEffectiveFrom { get; set; }
            public DateTime? StandardEffectiveTo { get; set; }
            public DateTime? StandardLastDateForNewStarts { get; set; }
            public bool IsValid { get; set; }
        }

        private static readonly TestDataForEffectiveFrom[] TestDateForEffectiveFrom =
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
            },
            new TestDataForEffectiveFrom
            {
                OrgStandardEffectiveFrom = DateTime.Today.AddDays(-1),
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = null,
                StandardLastDateForNewStarts = null,
                IsValid = false
            },
            new TestDataForEffectiveFrom
            {
                OrgStandardEffectiveFrom = DateTime.Today.AddDays(10),
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = DateTime.Today.AddDays(10),
                StandardLastDateForNewStarts = null,
                IsValid = true
            },
            new TestDataForEffectiveFrom
            {
                OrgStandardEffectiveFrom = DateTime.Today.AddDays(11),
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = DateTime.Today.AddDays(10),
                StandardLastDateForNewStarts = null,
                IsValid = false
            },
            new TestDataForEffectiveFrom
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
