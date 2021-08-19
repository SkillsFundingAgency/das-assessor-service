using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.ExternalApi.SubmitBatchCertificate
{
    public class WhenHandlingSubmitBatchCertificateRequest
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private Mock<IContactQueryRepository> _contactQueryRepository;
        private Mock<ILogger<SubmitBatchCertificateHandler>> _logger;

        private SubmitBatchCertificateHandler _handler;

        private Guid certId = Guid.NewGuid();
        private long uln = 12345678L;
        private int stdCode = 123;
        
        private SubmitBatchCertificateRequest _request;

        [SetUp]
        public void SetUp()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _contactQueryRepository = new Mock<IContactQueryRepository>();
            _logger = new Mock<ILogger<SubmitBatchCertificateHandler>>();

            _certificateRepository.Setup(m => m.GetCertificate(uln, stdCode))
                .ReturnsAsync(new Certificate()
            {
                Id = certId,
                Status = CertificateStatus.Approved,
                CertificateData = @"{}"
            });

            _certificateRepository.Setup(m => m.Update(It.Is<Certificate>(x => x.Id == certId), ExternalApiConstants.ApiUserName, CertificateActions.Submit, true, null))
            .ReturnsAsync(new Certificate()
            {
                Id = certId,
                Status = CertificateStatus.Submitted,
                CertificateData = @"{}"
            });


            _request = new SubmitBatchCertificateRequest()
            {
                StandardCode = stdCode,
                Uln = uln,
            };

            _handler = new SubmitBatchCertificateHandler(_certificateRepository.Object, _contactQueryRepository.Object, _logger.Object);
        }

        [Test]
        public async Task ThenReturnsCertificate()
        {
            // Arrange

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            //Assert
            result.Status.Should().Be(CertificateStatus.Submitted);
            result.UpdatedBy.Should().BeNull();
        }

        [Test]
        public async Task AndCertificateLogsAvailableThenReturnsCertificate()
        {
            // Arrange
            _certificateRepository.Setup(m => m.GetCertificateLogsFor(certId)).ReturnsAsync(new List<CertificateLog>()
            {
                new CertificateLog()
                {
                    EventTime = new DateTime(2021, 6, 1, 10, 30, 0),
                    Status = CertificateStatus.Draft,
                    Action = CertificateActions.Start,
                    Username = "Daphne"
                },
                new CertificateLog()
                {
                    EventTime = new DateTime(2021, 6, 1, 10, 31, 0),
                    Status = CertificateStatus.Draft,
                    Action = CertificateActions.Start,
                    Username = "Vilma"
                },
                new CertificateLog()
                {
                    EventTime = new DateTime(2021, 6, 1, 10, 32, 0),
                    Status = CertificateStatus.Submitted,
                    Action = CertificateActions.Start,
                    Username = "Fred"
                },
                new CertificateLog()
                {
                    EventTime = new DateTime(2021, 6, 1, 10, 33, 0),
                    Status = CertificateStatus.Submitted,
                    Action = CertificateActions.Start,
                    Username = "Shaggy"
                }
            });

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            //Assert
            result.Status.Should().Be(CertificateStatus.Submitted);
            result.CreatedBy.Should().Be("Vilma");
            result.UpdatedBy.Should().Be("Shaggy");
        }

        [Test]
        public async Task AndTheCertificateDoesNotExisitThenReturnsNull()
        {
            // Arrange
            _certificateRepository.Setup(m => m.GetCertificate(uln, stdCode))
                .ReturnsAsync((Certificate)null);

            // Act
            var result = await _handler.Handle(_request, CancellationToken.None);

            //Assert
            result.Should().BeNull();
        }
    }
}
