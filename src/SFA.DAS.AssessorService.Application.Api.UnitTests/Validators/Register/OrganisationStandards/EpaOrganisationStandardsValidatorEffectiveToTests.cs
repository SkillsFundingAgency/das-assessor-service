﻿using System;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Register.OrganisationStandards
{

    [TestFixture]
    public class EpaOrganisationStandardsValidatorEffectiveToTests
    {
        private EpaOrganisationValidator _validator;
        private Mock<IStringLocalizer<EpaOrganisationValidator>> _localizer;
        private Mock<IStandardService> _standardService;

        [SetUp]
        public void Setup()
        {
            _localizer = new Mock<IStringLocalizer<EpaOrganisationValidator>>();
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveToBeforeStandardEffectiveFrom])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveToBeforeStandardEffectiveFrom, "fail"));
            
            _standardService = new Mock<IStandardService>();
            _validator = new EpaOrganisationValidator(Mock.Of<IRegisterValidationRepository>(), Mock.Of<IRegisterQueryRepository>(), Mock.Of<ISpecialCharacterCleanserService>(), _localizer.Object, _standardService.Object);
        }

        [Test]
        public void RegisterValidationOfOrganisationStandardEffectiveFromAgainstStandardDetails(
            [ValueSource(nameof(TestDateForEffectiveTo))] TestDataForEffectiveTo testData)
        {
            var results = _validator.CheckOrganisationStandardToDateIsWithinStandardDateRanges(
                testData.OrgStandardEffectiveTo,
                testData.StandardEffectiveFrom,
                testData.StandardEffectiveTo);

            testData.IsValid.Should().Be(string.IsNullOrEmpty(results));
        }

        public class TestDataForEffectiveTo
        {
            public DateTime? OrgStandardEffectiveTo { get; set; }
            public DateTime StandardEffectiveFrom { get; set; }
            public DateTime? StandardEffectiveTo { get; set; }
            public bool IsValid { get; set; }
        }

        private static readonly TestDataForEffectiveTo[] TestDateForEffectiveTo =
        {
            new TestDataForEffectiveTo
            {
                OrgStandardEffectiveTo = null,
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = null,
                IsValid = true
            },
            new TestDataForEffectiveTo
            {
                OrgStandardEffectiveTo = DateTime.Today,
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = null,
                IsValid = true
            },
            new TestDataForEffectiveTo
            {
                OrgStandardEffectiveTo = DateTime.Today.AddDays(-1),
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = null,
                IsValid = false
            },
            new TestDataForEffectiveTo
            {
                OrgStandardEffectiveTo = DateTime.Today.AddDays(10),
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = DateTime.Today.AddDays(10),
                IsValid = true
            },
            new TestDataForEffectiveTo
            {
                OrgStandardEffectiveTo = DateTime.Today.AddDays(11),
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = DateTime.Today.AddDays(10),
                IsValid = true
            },
            new TestDataForEffectiveTo
            {
                OrgStandardEffectiveTo = DateTime.Today.AddDays(10),
                StandardEffectiveFrom = DateTime.Today,
                StandardEffectiveTo = DateTime.Today.AddDays(10),
                IsValid = true
            }
        };
    }
}
