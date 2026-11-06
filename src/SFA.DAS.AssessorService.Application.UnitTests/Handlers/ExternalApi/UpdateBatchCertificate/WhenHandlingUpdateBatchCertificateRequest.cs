using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.ExternalApi.UpdateBatchCertificate
{
    public class WhenHandlingUpdateBatchCertificateRequest
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private Mock<IContactQueryRepository> _contactQueryRepository;
        private Mock<ILogger<UpdateBatchCertificateHandler>> _logger;
        private Mock<IStandardService> _standardService;

        private UpdateBatchCertificateHandler _sut;

        private const long Uln = 12345678L;
        private const int StandardCode = 123;
        private const string StandardReference = "ST0123";
        private const string Version = "1.0";

        private readonly string _stdUId = $"{StandardReference}_{Version}";
        private readonly DateTime _achievementDate = new DateTime(2021, 7, 1, 0, 0, 0, DateTimeKind.Utc);

        private Certificate _certificate;
        private UpdateBatchCertificateRequest _request;

        [SetUp]
        public void SetUp()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _contactQueryRepository = new Mock<IContactQueryRepository>();
            _logger = new Mock<ILogger<UpdateBatchCertificateHandler>>();
            _standardService = new Mock<IStandardService>();

            _standardService
                .Setup(m => m.GetStandardVersionById(
                    _stdUId,
                    It.IsAny<string>()))
                .ReturnsAsync(new Standard
                {
                    StandardUId = _stdUId,
                    Title = "Standard Title"
                });

            _standardService
                .Setup(m => m.GetStandardOptionsByStandardId(_stdUId))
                .ReturnsAsync(new StandardOptions());

            _certificate = new Certificate
            {
                Status = CertificateStatus.Approved,
                CertificateData = new CertificateData()
            };

            _certificateRepository
                .Setup(m => m.GetCertificate(Uln, StandardCode))
                .ReturnsAsync(_certificate);

            _request = new UpdateBatchCertificateRequest
            {
                StandardReference = StandardReference,
                StandardCode = StandardCode,
                StandardUId = _stdUId,
                Uln = Uln,
                CertificateData = new CertificateData
                {
                    Version = Version,
                    AchievementDate = _achievementDate
                }
            };

            _sut = new UpdateBatchCertificateHandler(
                _certificateRepository.Object,
                _contactQueryRepository.Object,
                _logger.Object,
                _standardService.Object);
        }

        [Test]
        public async Task ThenReturnsCertificate()
        {
            // Arrange - common setup is provided by SetUp.

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            _certificateRepository.Verify(m =>
                m.UpdateStandardCertificate(
                    It.IsAny<Certificate>(),
                    ExternalApiConstants.ApiUserName,
                    CertificateActions.Amend,
                    true,
                    null));

            result.StandardUId.Should().Be(_stdUId);
            result.Status.Should().Be(CertificateStatus.Approved);
            result.CertificateData.EpaDetails.LatestEpaDate
                .Should().Be(_achievementDate);
        }

        [Test]
        public async Task AndNoAchievementDateThenReturnsCertificate()
        {
            // Arrange
            var request = new UpdateBatchCertificateRequest
            {
                StandardCode = StandardCode,
                StandardUId = _stdUId,
                Uln = Uln,
                CertificateData = new CertificateData()
            };

            // Act
            var result = await _sut.Handle(
                request,
                CancellationToken.None);

            // Assert
            _certificateRepository.Verify(m =>
                m.UpdateStandardCertificate(
                    It.IsAny<Certificate>(),
                    ExternalApiConstants.ApiUserName,
                    CertificateActions.Amend,
                    true,
                    null));

            result.CertificateData.EpaDetails.LatestEpaDate
                .Should().BeNull();
        }

        [Test]
        public async Task ThenStoredDeliveryDetailsArePreserved(
            [Values(
                CertificateSendTo.None,
                CertificateSendTo.Apprentice,
                CertificateSendTo.Employer)]
            CertificateSendTo storedSendTo,
            [Values(null, "", "Replacement")]
            string incomingDeliveryValue)
        {
            // Arrange
            _certificate.Status = CertificateStatus.Draft;

            var storedData = new CertificateData
            {
                SendTo = storedSendTo,
                ContactName = "Existing recipient",
                ContactOrganisation = "Existing organisation",
                Department = "Existing department",
                ContactAddLine1 = "1 Existing Road",
                ContactAddLine2 = "Existing building",
                ContactAddLine3 = "Existing district",
                ContactAddLine4 = "Existing city",
                ContactPostCode = "CV1 2WT"
            };

            _certificate.CertificateData = storedData;

            SetRequestDeliveryDetails(incomingDeliveryValue);

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            AssertDeliveryDetails(
                _certificate.CertificateData,
                storedData);

            AssertDeliveryDetails(
                result.CertificateData,
                storedData);

            _certificateRepository.Verify(m =>
                m.UpdateStandardCertificate(
                    _certificate,
                    ExternalApiConstants.ApiUserName,
                    CertificateActions.Amend,
                    true,
                    null),
                Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("Incoming")]
        public async Task AndNoStoredDeliveryDetailsThenIncomingDetailsAreIgnored(
            string incomingDeliveryValue)
        {
            // Arrange
            _certificate.Status = CertificateStatus.Draft;

            var expectedDeliveryDetails = new CertificateData();

            SetRequestDeliveryDetails(incomingDeliveryValue);

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            AssertDeliveryDetails(
                _certificate.CertificateData,
                expectedDeliveryDetails);

            AssertDeliveryDetails(
                result.CertificateData,
                expectedDeliveryDetails);

            _certificateRepository.Verify(m =>
                m.UpdateStandardCertificate(
                    _certificate,
                    ExternalApiConstants.ApiUserName,
                    CertificateActions.Amend,
                    true,
                    null),
                Times.Once);
        }

        [Test]
        public void AndThereIsNoOptionThenThrowNotFoundException()
        {
            // Arrange
            _standardService
                .Setup(m => m.GetStandardOptionsByStandardId(_stdUId))
                .ReturnsAsync((StandardOptions)null);

            // Act
            AsyncTestDelegate act = () =>
                _sut.Handle(_request, CancellationToken.None);

            // Assert
            Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Test]
        public void AndThereIsNoCertificateThenThrowNotFoundException()
        {
            // Arrange
            _certificateRepository
                .Setup(m => m.GetCertificate(Uln, StandardCode))
                .ReturnsAsync((Certificate)null);

            // Act
            AsyncTestDelegate act = () =>
                _sut.Handle(_request, CancellationToken.None);

            // Assert
            Assert.ThrowsAsync<NotFoundException>(act);
        }

        private void SetRequestDeliveryDetails(string value)
        {
            _request.CertificateData.SendTo = CertificateSendTo.Employer;
            _request.CertificateData.ContactName = value;
            _request.CertificateData.ContactOrganisation = value;
            _request.CertificateData.Department = value;
            _request.CertificateData.ContactAddLine1 = value;
            _request.CertificateData.ContactAddLine2 = value;
            _request.CertificateData.ContactAddLine3 = value;
            _request.CertificateData.ContactAddLine4 = value;
            _request.CertificateData.ContactPostCode = value;
        }

        private static void AssertDeliveryDetails(
            CertificateData actual,
            CertificateData expected)
        {
            actual.SendTo.Should().Be(expected.SendTo);
            actual.ContactName.Should().Be(expected.ContactName);
            actual.ContactOrganisation.Should()
                .Be(expected.ContactOrganisation);
            actual.Department.Should().Be(expected.Department);
            actual.ContactAddLine1.Should().Be(expected.ContactAddLine1);
            actual.ContactAddLine2.Should().Be(expected.ContactAddLine2);
            actual.ContactAddLine3.Should().Be(expected.ContactAddLine3);
            actual.ContactAddLine4.Should().Be(expected.ContactAddLine4);
            actual.ContactPostCode.Should().Be(expected.ContactPostCode);
        }
    }
}