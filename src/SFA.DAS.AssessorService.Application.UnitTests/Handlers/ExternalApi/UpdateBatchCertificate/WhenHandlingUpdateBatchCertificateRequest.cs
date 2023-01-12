using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.ExternalApi.UpdateBatchCertificate
{
    public class WhenHandlingUpdateBatchCertificateRequest
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private Mock<IContactQueryRepository> _contactQueryRepository;
        private Mock<ILogger<UpdateBatchCertificateHandler>> _logger;
        private Mock<IStandardService> _standardService;

        private UpdateBatchCertificateHandler _handler;

        private long uln = 12345678L;
        private int stdCode = 123;
        private string stdUId = "ST0123_1.0";
        private DateTime achievementDate = new DateTime(2021, 7, 1);
        private UpdateBatchCertificateRequest _request;

        [SetUp]
        public void SetUp()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _contactQueryRepository = new Mock<IContactQueryRepository>();
            _logger = new Mock<ILogger<UpdateBatchCertificateHandler>>();
            _standardService = new Mock<IStandardService>();

            _standardService.Setup(m => m.GetStandardOptionsByStandardId(stdUId)).ReturnsAsync(new StandardOptions());

            _certificateRepository.Setup(m => m.GetCertificate(uln, stdCode)).ReturnsAsync(new Certificate()
            {
                Status = CertificateStatus.Approved,
                CertificateData = @"{}"
            });

            _request = new UpdateBatchCertificateRequest()
            {
                StandardCode = stdCode,
                StandardUId = stdUId,
                Uln = uln,
                CertificateData = new Domain.JsonData.CertificateData()
                {
                    AchievementDate = achievementDate
                }
            };

            _handler = new UpdateBatchCertificateHandler(_certificateRepository.Object, _contactQueryRepository.Object, _logger.Object, _standardService.Object);
        }

        [Test]
        public async Task ThenReturnsCertificate()
        {
            // Arrange

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);


            //Assert
            _certificateRepository.Verify(m => m.Update(It.IsAny<Certificate>(), ExternalApiConstants.ApiUserName, CertificateActions.Amend, true, null));

            result.StandardUId.Should().Be(stdUId);
            result.Status.Should().Be(CertificateStatus.Approved);

            var certData = JsonConvert.DeserializeObject<CertificateData>(result.CertificateData);
            certData.EpaDetails.LatestEpaDate.Should().Be(achievementDate);
        }

        [Test]
        public async Task AndNoAchievementDateThenReturnsCertificate()
        {
            // Arrange
            var request = new UpdateBatchCertificateRequest()
            {
                StandardCode = stdCode,
                StandardUId = stdUId,
                Uln = uln,
                CertificateData = new Domain.JsonData.CertificateData()
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);


            //Assert
            _certificateRepository.Verify(m => m.Update(It.IsAny<Certificate>(), ExternalApiConstants.ApiUserName, CertificateActions.Amend, true, null));

            var certData = JsonConvert.DeserializeObject<CertificateData>(result.CertificateData);
            certData.EpaDetails.LatestEpaDate.Should().BeNull();
        }

        [Test]
        public void AndThereIsNoOptionThenThrowNotFoundException()
        {
            // Arrange
            _standardService.Setup(m => m.GetStandardOptionsByStandardId(stdUId)).ReturnsAsync((StandardOptions)null);

            // Act
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(_request, CancellationToken.None));
        }

        [Test]
        public void AndThereIsNoCertificateThenThrowNotFoundException()
        {
            // Arrange
            _certificateRepository.Setup(m => m.GetCertificate(uln, stdCode)).ReturnsAsync((Certificate)null);

            // Act
            Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(_request, CancellationToken.None));
        }
    }
}
