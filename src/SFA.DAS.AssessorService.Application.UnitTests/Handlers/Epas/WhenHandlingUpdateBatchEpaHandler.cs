using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Epas
{
    public class WhenHandlingUpdateBatchEpaHandler
    {
        UpdateBatchEpaHandler _sut;
        Mock<ICertificateRepository> _mockCertificateRepository;

        [SetUp]
        public void Setup()
        {
            _mockCertificateRepository = new Mock<ICertificateRepository>();
            _sut = new UpdateBatchEpaHandler(_mockCertificateRepository.Object, Mock.Of<ILogger<UpdateBatchEpaHandler>>());
        }

        [Test, MoqAutoData]
        public void AndCertificateNotFound_ThrowsNotFound(UpdateBatchEpaRequest request)
        {
            //Arrange
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);

            //Act
            Func<Task> act = async () => { await _sut.Handle(request, new CancellationToken()); };

            //Assert
            act.Should().Throw<NotFoundException>();
        }

        [Test, RecursiveMoqAutoData]
        public async Task AndCertificateFound_UpdatesEpaDetailsFromRequest_AndReturnsResult(
            UpdateBatchEpaRequest request,
            Certificate certificate,
            CertificateData certificateData)
        {
            //Arrange
            certificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync(certificate);

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            var latestEpa = request.EpaDetails.Epas.OrderByDescending(s => s.EpaDate).FirstOrDefault();
            result.LatestEpaDate.Should().Be(latestEpa.EpaDate);
            result.LatestEpaOutcome.Should().Be(latestEpa.EpaOutcome);
            result.EpaReference.Should().Be(certificate.CertificateReference);
            result.Epas.Should().BeEquivalentTo(request.EpaDetails.Epas);

            _mockCertificateRepository.Verify(s => s.Update(It.IsAny<Certificate>(), "API", It.IsAny<string>(), true, null), Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task AndCertificateFound_SetsCertDataForFail(
            UpdateBatchEpaRequest request,
            Certificate certificate,
            CertificateData certificateData)
        {
            //Arrange
            certificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync(certificate);
            var latestEpa = request.EpaDetails.Epas.OrderByDescending(s => s.EpaDate).FirstOrDefault();
            latestEpa.EpaOutcome = EpaOutcome.Fail;
            var epaAction = CertificateActions.Submit;

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            var data = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            data.AchievementDate.Should().Be(latestEpa.EpaDate);
            data.OverallGrade.Should().Be(CertificateGrade.Fail);
            certificate.Status.Should().Be(CertificateStatus.Submitted);
            _mockCertificateRepository.Verify(s => s.Update(It.IsAny<Certificate>(), "API", epaAction, true, null), Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task AndCertificateFound_SetsCertDataForPass(
            UpdateBatchEpaRequest request,
            Certificate certificate,
            CertificateData certificateData)
        {
            //Arrange
            certificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync(certificate);
            var latestEpa = request.EpaDetails.Epas.OrderByDescending(s => s.EpaDate).FirstOrDefault();
            latestEpa.EpaOutcome = EpaOutcome.Pass;
            var epaAction = CertificateActions.Epa;

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            var data = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            data.AchievementDate.Should().BeNull();
            data.OverallGrade.Should().BeNull();
            certificate.Status.Should().Be(CertificateStatus.Draft);
            _mockCertificateRepository.Verify(s => s.Update(It.IsAny<Certificate>(), "API", epaAction, true, null), Times.Once);
        }
    }
}
