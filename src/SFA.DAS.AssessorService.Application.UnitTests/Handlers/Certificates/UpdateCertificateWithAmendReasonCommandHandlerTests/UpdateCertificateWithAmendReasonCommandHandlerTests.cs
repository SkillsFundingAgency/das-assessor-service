using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Enums;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificateWithAmendReasonCommandHandlerTests
{
    public class UpdateCertificateWithAmendReasonCommandHandlerTests
    {
        [Test]
        public async Task WhenCalled_ThenGetCertificateIsCalled_ForCertificateReference()
        {
            // Arrange
            var fixture = new UpdateCertificateWithAmendReasonCommandHandlerTestsFixture()
                .WithCertificate("12345");

            // Act
            await fixture.Handle(new UpdateCertificateWithAmendReasonCommand 
            { 
                CertificateReference = "12345", IncidentNumber = "333", 
                Reasons = AmendReasons.ApprenticeDetails | AmendReasons.ApprenticeAddress,
                OtherReason = string.Empty,
                Username = "Some User"
            });

            // Assert
            fixture.VerifyGetCertificateCalled("12345");
            fixture.VerifyUpdateCalled("12345", "333", new List<string> { AmendReasons.ApprenticeDetails.ToString(), AmendReasons.ApprenticeAddress.ToString()}, "Some User", CertificateActions.AmendReason, string.Empty);
        }

        public class UpdateCertificateWithAmendReasonCommandHandlerTestsFixture
        {
            protected Mock<ICertificateRepository> _certificateRepository;
            protected Mock<ILogger<UpdateCertificateWithAmendReasonCommandHandler>> _logger;

            protected UpdateCertificateWithAmendReasonCommandHandler _sut;

            public UpdateCertificateWithAmendReasonCommandHandlerTestsFixture()
            {
                _certificateRepository = new Mock<ICertificateRepository>();
                _logger = new Mock<ILogger<UpdateCertificateWithAmendReasonCommandHandler>>();

                _sut = new UpdateCertificateWithAmendReasonCommandHandler(_certificateRepository.Object, _logger.Object);
            }

            public UpdateCertificateWithAmendReasonCommandHandlerTestsFixture WithCertificate(string reference)
            {
                _certificateRepository.Setup(r => r.GetCertificate(reference))
                    .Returns(Task.FromResult(
                        new Certificate
                        {
                            Id = Guid.NewGuid(),
                            CertificateReference = reference,
                            CertificateData = JsonConvert.SerializeObject(new CertificateData() { FullName = "Joe Bloggs" })
                        }));

                return this;
            }

            public async Task Handle(UpdateCertificateWithAmendReasonCommand command)
            {
                await _sut.Handle(command, new CancellationToken());
            }

            public void VerifyGetCertificateCalled(string certificateReference)
            {
                _certificateRepository.Verify(r => r.GetCertificate(certificateReference), Times.Once);
            }

            public void VerifyUpdateCalled(string certificateReference, string incidentNumber, List<string> AmendReasons, string userName, 
                string action, string reasonForChange)
            {
                _certificateRepository.Verify(r => r.Update(
                    It.Is<Certificate>(c => c.CertificateReference == certificateReference
                        && JsonConvert.DeserializeObject<CertificateData>(c.CertificateData).IncidentNumber == incidentNumber
                        && JsonConvert.DeserializeObject<CertificateData>(c.CertificateData).AmendReasons.SequenceEqual(AmendReasons)), userName,
                    action, true, reasonForChange), Times.Once);
            }
        }
    }
}
