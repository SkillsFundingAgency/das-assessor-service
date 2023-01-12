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
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Epas
{
    public class WhenHandlingDeleteBatchEpaHandler
    {
        DeleteBatchEpaHandler _sut;
        Mock<ICertificateRepository> _mockCertificateRepository;

        [SetUp]
        public void Setup()
        {
            _mockCertificateRepository = new Mock<ICertificateRepository>();
            _sut = new DeleteBatchEpaHandler(_mockCertificateRepository.Object, Mock.Of<ILogger<DeleteBatchEpaHandler>>());
        }

        [Test, RecursiveMoqAutoData]
        public void AndCertificateNotFound_ThrowsNotFound(
            DeleteBatchEpaRequest request)
        {
            //Arrange
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync((Certificate)null);

            //Act
            Func<Task> act = async () => { await _sut.Handle(request, new CancellationToken()); };

            //Assert
            act.Should().Throw<NotFoundException>();
        }

        [Test, RecursiveMoqAutoData]
        public async Task AndCertificateFound_DeletesEPARecordInformation_And_DeletesCertificate(
            DeleteBatchEpaRequest request,
            CertificateData certificateData,
            Certificate certificate)
        {
            //Arrange
            certificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            _mockCertificateRepository.Setup(s => s.GetCertificate(request.Uln, request.StandardCode)).ReturnsAsync(certificate);
            var certificateAction = CertificateActions.Epa;

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            _mockCertificateRepository.Verify(s => s.Update(It.IsAny<Certificate>(), "API", certificateAction, true, null), Times.Once);
            var modifiedCertificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            modifiedCertificateData.EpaDetails.Should().BeEquivalentTo(new EpaDetails { Epas = new System.Collections.Generic.List<EpaRecord>() });

            _mockCertificateRepository.Verify(v => v.Delete(request.Uln, request.StandardCode, "API", CertificateActions.Delete, true, null, null));
        }


    }
}
