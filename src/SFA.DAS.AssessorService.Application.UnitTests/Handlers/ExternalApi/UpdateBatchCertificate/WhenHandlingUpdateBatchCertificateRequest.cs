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

        private UpdateBatchCertificateHandler _handler;

        private const long Uln = 12345678L;
        private const int StandardCode = 123;
        private const string StandardReference = "ST0123";
        private const string Version = "1.0";

        private string _stdUId = $"{StandardReference}_{Version}";
        private DateTime _achievementDate = new DateTime(2021, 7, 1);

        private UpdateBatchCertificateRequest _request;

        [SetUp]
        public void SetUp()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _contactQueryRepository = new Mock<IContactQueryRepository>();
            _logger = new Mock<ILogger<UpdateBatchCertificateHandler>>();
            _standardService = new Mock<IStandardService>();

            _standardService
                .Setup(m => m.GetStandardVersionById(_stdUId, It.IsAny<string>()))
                .ReturnsAsync(new Standard { StandardUId = _stdUId, Title = "Standard Title" });

            _standardService
                .Setup(m => m.GetStandardOptionsByStandardId(_stdUId))
                .ReturnsAsync(new StandardOptions());

            _certificateRepository
                .Setup(m => m.GetCertificate(Uln, StandardCode))
                .ReturnsAsync(new Certificate()
                {
                    Status = CertificateStatus.Approved,
                    CertificateData = new CertificateData()
                });

            _request = new UpdateBatchCertificateRequest()
            {
                StandardReference = StandardReference,
                StandardCode = StandardCode,
                StandardUId = _stdUId,
                Uln = Uln,
                CertificateData = new CertificateData()
                {
                    Version = Version,
                    AchievementDate = _achievementDate
                }
            };

            _handler = new UpdateBatchCertificateHandler(_certificateRepository.Object, _contactQueryRepository.Object, _logger.Object, _standardService.Object);
        }

        [Test]
        public async Task ThenReturnsCertificate()
        {
            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            // Assert
            _certificateRepository.Verify(m => m.UpdateStandardCertificate(It.IsAny<Certificate>(), ExternalApiConstants.ApiUserName, CertificateActions.Amend, true, null));

            result.StandardUId.Should().Be(_stdUId);
            result.Status.Should().Be(CertificateStatus.Approved);
            result.CertificateData.EpaDetails.LatestEpaDate.Should().Be(_achievementDate);
        }

        [Test]
        public async Task AndNoAchievementDateThenReturnsCertificate()
        {
            // Arrange
            var request = new UpdateBatchCertificateRequest()
            {
                StandardCode = StandardCode,
                StandardUId = _stdUId,
                Uln = Uln,
                CertificateData = new Domain.JsonData.CertificateData()
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _certificateRepository.Verify(m => m.UpdateStandardCertificate(It.IsAny<Certificate>(), ExternalApiConstants.ApiUserName, CertificateActions.Amend, true, null));
            result.CertificateData.EpaDetails.LatestEpaDate.Should().BeNull();
        }

        [Test]
        public void AndThereIsNoOptionThenThrowNotFoundException()
        {
            // Arrange
            _standardService.Setup(m => m.GetStandardOptionsByStandardId(_stdUId)).ReturnsAsync((StandardOptions)null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(_request, CancellationToken.None));
        }

        [Test]
        public void AndThereIsNoCertificateThenThrowNotFoundException()
        {
            // Arrange
            _certificateRepository.Setup(m => m.GetCertificate(Uln, StandardCode)).ReturnsAsync((Certificate)null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(_request, CancellationToken.None));
        }
    }
}
