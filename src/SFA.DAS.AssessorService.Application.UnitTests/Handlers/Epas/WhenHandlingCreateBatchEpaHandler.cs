using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Epas;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Epas
{
    public class WhenHandlingCreateBatchEpaHandler
    {
        CreateBatchEpaHandler _sut;
        Mock<ICertificateRepository> _mockCertificateRepository;
        Mock<IMediator> _mockMediator;

        [SetUp]
        public void Setup()
        {
            _mockCertificateRepository = new Mock<ICertificateRepository>();
            _mockMediator = new Mock<IMediator>();
            _sut = new CreateBatchEpaHandler(_mockCertificateRepository.Object, Mock.Of<ILogger<CreateBatchEpaHandler>>(), _mockMediator.Object);
        }

        [Test, RecursiveMoqAutoData]
        public async Task AndCertificateNotFound_StartsNewCertificateRequest(
            CreateBatchEpaRequest request,
            CertificateData certificateData,
            Certificate certificate)
        {
            //Arrange
            certificate.CertificateData = certificateData;
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockMediator.Setup(s => s.Send(It.IsAny<StartCertificateRequest>(), new CancellationToken())).ReturnsAsync(certificate);
            
            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            _mockMediator.Verify(v => v.Send(It.Is<StartCertificateRequest>(
                r => r.StandardCode == request.StandardCode && r.UkPrn == request.UkPrn &&
                r.Uln == request.Uln && r.Username == "API" && r.CourseOption == request.CourseOption &&
                r.StandardUId == request.StandardUId), new CancellationToken()), Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task AndCertificateNotFound_StartsNewCertificateRequest_ReturnsNull_ThrowsException(
            CreateBatchEpaRequest request)
        {
            // Arrange
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockMediator.Setup(s => s.Send(It.IsAny<StartCertificateRequest>(), new CancellationToken())).ReturnsAsync((Certificate)null);

            // Act
            Func<Task> act = async () => { await _sut.Handle(request, new CancellationToken()); };

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Test, RecursiveMoqAutoData]
        public async Task AndCreatesNewCertificate_UpdatesEpaDetails(
            CreateBatchEpaRequest request,
            CertificateData certificateData,
            Certificate certificate)
        {
            //Arrange
            // Reset EpaDetails
            certificateData.EpaDetails = new EpaDetails { Epas = new System.Collections.Generic.List<EpaRecord>() };
            certificate.CertificateData = certificateData;
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockMediator.Setup(s => s.Send(It.IsAny<StartCertificateRequest>(), new CancellationToken())).ReturnsAsync(certificate);

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            var latestEpa = request.EpaDetails.Epas.OrderByDescending(s => s.EpaDate).FirstOrDefault();
            result.LatestEpaDate.Should().Be(latestEpa.EpaDate);
            result.LatestEpaOutcome.Should().Be(latestEpa.EpaOutcome);
            result.Epas.Should().BeEquivalentTo(request.EpaDetails.Epas);

            _mockCertificateRepository.Verify(s => s.UpdateStandardCertificate(It.IsAny<Certificate>(), "API", It.IsAny<string>(), true, null), Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task AndCreatesNewCertificate_SetsCertificateDataForPass(
            CreateBatchEpaRequest request,
            CertificateData certificateData,
            Certificate certificate)
        {
            //Arrange
            certificate.CertificateData = certificateData;
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockMediator.Setup(s => s.Send(It.IsAny<StartCertificateRequest>(), new CancellationToken())).ReturnsAsync(certificate);
            var latestEpa = request.EpaDetails.Epas.OrderByDescending(s => s.EpaDate).FirstOrDefault();
            latestEpa.EpaOutcome = EpaOutcome.Pass;
            var epaAction = CertificateActions.Epa;

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            certificate.CertificateData.AchievementDate.Should().BeNull();
            certificate.CertificateData.OverallGrade.Should().BeNull();
            certificate.Status.Should().Be(CertificateStatus.Draft);
            _mockCertificateRepository.Verify(s => s.UpdateStandardCertificate(It.IsAny<Certificate>(), "API", epaAction, true, null), Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task AndCreatesNewCertificate_SetsCertificateDataForFail(
            CreateBatchEpaRequest request,
            CertificateData certificateData,
            Certificate certificate)
        {
            //Arrange
            certificateData.EpaDetails = new EpaDetails { Epas = new List<EpaRecord>() }; // reset details for fail.
            certificate.CertificateData = certificateData;
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);
            _mockMediator.Setup(s => s.Send(It.IsAny<StartCertificateRequest>(), new CancellationToken())).ReturnsAsync(certificate);
            var latestEpa = request.EpaDetails.Epas.OrderByDescending(s => s.EpaDate).FirstOrDefault();
            latestEpa.EpaOutcome = EpaOutcome.Fail;
            var epaAction = CertificateActions.Submit;

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            certificate.CertificateData.AchievementDate.Should().Be(latestEpa.EpaDate);
            certificate.CertificateData.OverallGrade.Should().Be(latestEpa.EpaOutcome);
            certificate.Status.Should().Be(CertificateStatus.Submitted);
            _mockCertificateRepository.Verify(s => s.UpdateStandardCertificate(It.IsAny<Certificate>(), "API", epaAction, true, null), Times.Once);
        }
    }
}
