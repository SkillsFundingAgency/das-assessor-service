using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatePrintRequestHandlerTests
{
    [TestFixture]
    public class UpdateCertificatePrintRequestHandlerTests
    {
        private Guid CertificateId = Guid.NewGuid();

        [Test]
        public async Task WhenValidRequest_ThenUpdatesCertificateAndLogs()
        {
            // Arrange
            var fixture = new TheFixture().WithCertificate(CertificateId, EpaOutcome.Pass, CertificateStatus.Submitted);
            var request = new UpdateCertificatePrintRequestCommand
            {
                CertificateId = CertificateId,
                PrintRequestedAt = new DateTime(2021, 1, 1),
                PrintRequestedBy = "Apprentice",
                Address = new CertificatePrintAddress
                {
                    ContactName = "Contact Name",
                    ContactOrganisation = "Org",
                    ContactAddLine1 = "Line1",
                    ContactPostCode = "PC"
                }
            };

            // Act
            await fixture.Handle(request);

            // Assert
            fixture.VerifyUpdated(request);
        }

        [TestCase(EpaOutcome.Fail, CertificateStatus.Submitted, null, TestName = "WhenEpaOutcomeIsNotPass_ThenThrowsArgumentException")]
        [TestCase(EpaOutcome.Pass, CertificateStatus.Draft, null, TestName = "WhenStatusIsNotSubmitted_ThenThrowsArgumentException")]
        public void WhenCertificateIsNotInValidState_ThenThrowsArgumentException(string epaOutcome, string status, string printRequestedAt)
        {
            // Arrange
            var fixture = new TheFixture().WithCertificate(CertificateId, epaOutcome, status, printRequestedAt != null ? DateTime.Parse(printRequestedAt) : (DateTime?)null);
            var request = new UpdateCertificatePrintRequestCommand
            {
                CertificateId = CertificateId,
                PrintRequestedAt = new DateTime(2021, 1, 1),
                PrintRequestedBy = "Apprentice",
                Address = new CertificatePrintAddress
                {
                    ContactName = "Contact Name",
                    ContactOrganisation = "Org",
                    ContactAddLine1 = "Line1",
                    ContactPostCode = "PC"
                }
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => fixture.Handle(request));
        }

        private class TheFixture
        {
            private Mock<IMediator> _mediator;
            private Mock<ICertificateRepository> _certificateRepository;
            private UpdateCertificatePrintRequestHandler _sut;
            private Certificate _certificate;


            public TheFixture()
            {
                _mediator = new Mock<IMediator>();
                _certificateRepository = new Mock<ICertificateRepository>();
                _sut = new UpdateCertificatePrintRequestHandler(_mediator.Object, _certificateRepository.Object, new Mock<ILogger<UpdateCertificatePrintRequestHandler>>().Object);
            }

            public TheFixture WithCertificate(Guid id, string latestEpaOutcome, string status, DateTime? printRequestedAt = null)
            {
                _certificate = new Certificate
                {
                    Id = id,
                    Status = status,
                    PrintRequestedAt = printRequestedAt,
                    CertificateData = new Domain.JsonData.CertificateData
                    {
                        EpaDetails = new Domain.JsonData.EpaDetails { LatestEpaOutcome = latestEpaOutcome }
                    }
                };

                typeof(CertificateBase)
                    .GetProperty(nameof(CertificateBase.LatestEPAOutcome))
                    .SetValue(_certificate, latestEpaOutcome);

                _mediator.Setup(m => m.Send(It.Is<GetCertificateRequest>(r => r.CertificateId == id), It.IsAny<CancellationToken>())).ReturnsAsync(_certificate);
                return this;
            }

            public async Task Handle(UpdateCertificatePrintRequestCommand request)
            {
                await _sut.Handle(request, CancellationToken.None);
            }

            public void VerifyUpdated(UpdateCertificatePrintRequestCommand request)
            {
                var expectedActor = request.Address.ContactName;

                Assert.IsNotNull(_certificate);
                Assert.AreEqual(request.PrintRequestedAt, _certificate.PrintRequestedAt);
                Assert.AreEqual(request.PrintRequestedBy, _certificate.PrintRequestedBy);
                Assert.AreEqual(CertificateStatus.PrintRequested, _certificate.Status);
                Assert.AreEqual(request.Address.ContactName, _certificate.CertificateData.ContactName);
                Assert.AreEqual(request.Address.ContactAddLine1, _certificate.CertificateData.ContactAddLine1);
                Assert.AreEqual(request.Address.ContactPostCode, _certificate.CertificateData.ContactPostCode);
            }
        }
    }
}
