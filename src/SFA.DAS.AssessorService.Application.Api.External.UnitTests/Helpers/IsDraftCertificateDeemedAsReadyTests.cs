using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.External.Helpers;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.External.UnitTests.Helpers
{
    public class IsDraftCertificateDeemedAsReadyTests
    {
        private Certificate _certificate;
        private IEnumerable<string> _potentialOptions;
        
        [SetUp]
        public void Arrange()
        {
            var autoFixture = new Fixture();

            _potentialOptions = autoFixture.Create<IEnumerable<string>>();

            // Create a draft certificate with valid uln and selected option
            _certificate = autoFixture.Create<Certificate>();
            _certificate.Status.CurrentStatus = CertificateStatus.Draft;
            _certificate.CertificateData.Learner.Uln = 1000000001;
            _certificate.CertificateData.LearningDetails.CourseOption = _potentialOptions.First();
        }

        [Test]
        public void When_AllRequiredFieldsAreComplete_Then_ReturnTrue()
        {
            var result = CertificateHelpers.IsDraftCertificateDeemedAsReady(_certificate);

            result.Should().BeTrue();
        }

        [Test]
        public void When_CertificateDataIsNull_Then_ReturnFalse()
        {
            _certificate.CertificateData = null;

            CheckHelperReturnsFalse();
        }

        [Test]
        public void When_CertificateStatusIsNotDraft_Then_ReturnFalse()
        {
            _certificate.Status.CurrentStatus = CertificateStatus.Submitted;

            CheckHelperReturnsFalse();
        }

        [Test]
        public void When_CertificateReferenceIsNull_Then_ReturnFalse()
        {
            _certificate.CertificateData.CertificateReference = null;

            CheckHelperReturnsFalse();
        }

        [Test]
        public void When_CertificateStandardIsNull_Then_ReturnFalse()
        {
            _certificate.CertificateData.Standard = null;

            CheckHelperReturnsFalse();
        }

        [Test]
        public void When_PotentialOptionsIsNotNull_And_CourseOptionIsIncorrectOrNull_Then_ReturnFalse()
        {
            _certificate.CertificateData.LearningDetails.CourseOption = null;

            CheckHelperReturnsFalse();
        }

        [Test]
        public void When_PostalContactIsNull_Then_ReturnFalse()
        {
            _certificate.CertificateData.PostalContact = null;

            CheckHelperReturnsFalse();
        }

        [Test]
        public void When_ContactNameIsNull_Then_ReturnFalse()
        {
            _certificate.CertificateData.PostalContact.ContactName = null;

            CheckHelperReturnsFalse();
        }

        [Test]
        public void When_CityIsNull_Then_ReturnFalse()
        {
            _certificate.CertificateData.PostalContact.City = null;

            CheckHelperReturnsFalse();
        }

        [Test]
        public void When_PostCodeIsNull_Then_ReturnFalse()
        {
            _certificate.CertificateData.PostalContact.PostCode = null;

            CheckHelperReturnsFalse();
        }

        [Test]
        public void When_FamilyNameIsNull_Then_ReturnFalse()
        {
            _certificate.CertificateData.Learner.FamilyName = null;

            CheckHelperReturnsFalse();
        }

        [TestCase(1)]
        [TestCase(123456789)]
        [TestCase(12345678901)]
        public void When_UlnIsNot10Digits_Then_ReturnFalse(long uln)
        {
            _certificate.CertificateData.Learner.Uln = uln;

            CheckHelperReturnsFalse();
        }

        [Test]
        public void When_OverallGradeIsNull_Then_ReturnFalse()
        {
            _certificate.CertificateData.LearningDetails.OverallGrade = null;

            CheckHelperReturnsFalse();
        }

        [Test]
        public void When_AchievementDateIsNull_Then_ReturnFalse()
        {
            _certificate.CertificateData.LearningDetails.AchievementDate = null;

            CheckHelperReturnsFalse();
        }

        private void CheckHelperReturnsFalse()
        {
            var result = CertificateHelpers.IsDraftCertificateDeemedAsReady(_certificate, hasOptions: true);

            result.Should().BeFalse();
        }
    }
}
