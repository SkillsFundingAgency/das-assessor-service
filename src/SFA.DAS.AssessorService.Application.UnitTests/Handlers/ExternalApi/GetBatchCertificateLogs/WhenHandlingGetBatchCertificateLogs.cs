using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.ExternalApi.GetBatchCertificateLogs
{
    public class WhenHandlingGetBatchCertificateLogs
    {
        private GetBatchCertificateLogsRequest _request;

        private Mock<ICertificateRepository> _mockCertificateRepository;
        private Certificate _certificate;
        private List<CertificateLog> _certificateLogs;
        private GetBatchCertificateLogsHandler _handler;

        [SetUp]
        public void Setup()
        {
            _request = Builder<GetBatchCertificateLogsRequest>.CreateNew().Build();

            _certificate = Builder<Certificate>.CreateNew().Build();

            _certificateLogs = Builder<CertificateLog>.CreateListOfSize(3)
                .All()
                    .With(x => x.CertificateId = _certificate.Id)
                .Build().ToList();

            _mockCertificateRepository = new Mock<ICertificateRepository>();

            _mockCertificateRepository.Setup(r => r.GetCertificate(It.IsAny<string>()))
                .ReturnsAsync(_certificate);

            _mockCertificateRepository.Setup(r => r.GetCertificateLogsFor(_certificate.Id))
                .ReturnsAsync(_certificateLogs);

            _handler = new GetBatchCertificateLogsHandler(_mockCertificateRepository.Object);
        }

        [Test]
        public async Task ThenGetCertificateIsCalled()
        {
            await _handler.Handle(_request, It.IsAny<CancellationToken>());

            _mockCertificateRepository.Verify(r => r.GetCertificate(_request.CertificateReference), Times.Once);
        }

        [Test]
        public async Task ThenGetCertificateLogsIsCalled()
        {
            await _handler.Handle(_request, It.IsAny<CancellationToken>());

            _mockCertificateRepository.Verify(r => r.GetCertificateLogsFor(_certificate.Id), Times.Once);
        }

        [Test]
        public async Task ThenReturnCertificateLogsResponse()
        {
            var result = await _handler.Handle(_request, It.IsAny<CancellationToken>());

            result.CertificateLogs.Should().HaveCount(_certificateLogs.Count);
        }
    }
}
