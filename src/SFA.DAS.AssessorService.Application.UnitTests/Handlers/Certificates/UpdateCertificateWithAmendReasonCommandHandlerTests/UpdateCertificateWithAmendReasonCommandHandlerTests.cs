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
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificateWithAmendReasonCommandHandlerTests
{
    public class UpdateCertificateWithAmendReasonCommandHandlerTests
    {
        [Test, MoqAutoData]
        public async Task WhenCalled_ThenGetCertificateIsCalled_ForCertificateReference(UpdateCertificateWithAmendReasonCommand command)
        {
            // Arrange
            var fixture = new UpdateCertificateWithAmendReasonCommandHandlerTestsFixture()
                .WithCertificate(command.CertificateReference);

            // Act
            await fixture.Handle(new UpdateCertificateWithAmendReasonCommand 
            { 
                CertificateReference = command.CertificateReference, 
                IncidentNumber = command.IncidentNumber, 
                Reasons = command.Reasons,
                OtherReason = command.OtherReason,
                Username = command.Username
            });

            // Assert
            fixture.VerifyGetCertificateCalled(command.CertificateReference);
        }

        [Test, MoqAutoData]
        public void WhenCalledWithInvalidCertificate_ThenThrowsNotFoundException(UpdateCertificateWithAmendReasonCommand command)
        {
            // Arrange
            var fixture = new UpdateCertificateWithAmendReasonCommandHandlerTestsFixture()
                .WithCertificate(command.CertificateReference);

            command.OtherReason = string.Empty;

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await
                fixture.Handle(new UpdateCertificateWithAmendReasonCommand
                {
                    CertificateReference = command.CertificateReference + "NotValid",
                    IncidentNumber = command.IncidentNumber,
                    Reasons = command.Reasons,
                    OtherReason = command.OtherReason,
                    Username = command.Username
                }));
        }

        [Test, MoqAutoData]
        public async Task WhenCalledWithoutOtherReason_ThenUpdateIsCalled_ForCertificateReference(UpdateCertificateWithAmendReasonCommand command)
        {
            // Arrange
            var fixture = new UpdateCertificateWithAmendReasonCommandHandlerTestsFixture()
                .WithCertificate(command.CertificateReference);

            command.OtherReason = string.Empty;

            // Act
            await fixture.Handle(new UpdateCertificateWithAmendReasonCommand
            {
                CertificateReference = command.CertificateReference,
                IncidentNumber = command.IncidentNumber,
                Reasons = command.Reasons,
                OtherReason = command.OtherReason,
                Username = command.Username
            });

            // Assert
            fixture.VerifyUpdateCalled(command.CertificateReference, command.IncidentNumber,
                command.Reasons, command.Username, CertificateActions.AmendReason, command.OtherReason);
        }

        [Test, MoqAutoData]
        public async Task WhenCalledWithOtherReason_ThenUpdateIsCalled_ForCertificateReference(UpdateCertificateWithAmendReasonCommand command)
        {
            // Arrange
            var fixture = new UpdateCertificateWithAmendReasonCommandHandlerTestsFixture()
                .WithCertificate(command.CertificateReference);

            command.OtherReason = "Some other reason";

            // Act
            await fixture.Handle(new UpdateCertificateWithAmendReasonCommand
            {
                CertificateReference = command.CertificateReference,
                IncidentNumber = command.IncidentNumber,
                Reasons = command.Reasons,
                OtherReason = command.OtherReason,
                Username = command.Username
            });

            // Assert
            fixture.VerifyUpdateCalled(command.CertificateReference, command.IncidentNumber, 
                command.Reasons, command.Username, CertificateActions.AmendReason, command.OtherReason);
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

            public void VerifyUpdateCalled(string certificateReference, string incidentNumber, AmendReasons? reasons, string userName, 
                string action, string reasonForChange)
            {
                var amendReasons = reasons?.ToString().Split(',').Select(p => p.Trim()).ToList() ?? new List<string>();
                
                _certificateRepository.Verify(r => r.Update(
                    It.Is<Certificate>(c => c.CertificateReference == certificateReference
                        && JsonConvert.DeserializeObject<CertificateData>(c.CertificateData).IncidentNumber == incidentNumber
                        && JsonConvert.DeserializeObject<CertificateData>(c.CertificateData).AmendReasons.SequenceEqual(amendReasons)), userName,
                    action, true, reasonForChange), Times.Once);
            }
        }
    }
}
