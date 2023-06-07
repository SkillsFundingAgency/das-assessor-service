using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Threading;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Application.Handlers.EmailHandlers;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Api.Types.Consts;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.EmailHandler
{
    [TestFixture]
    public class SendAddStandardEmailHandlerTests
    {
        private Mock<IEMailTemplateQueryRepository> _mockEmailTemplateRepo;
        private Mock<IContactQueryRepository> _mockContactRepo;
        private Mock<IStandardService> _mockStandardService;
        private Mock<IMediator> _mockMediator;
        private Mock<ILogger<SendAddStandardEmailHandler>> _mockLogger; 
        private SendAddStandardEmailHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockEmailTemplateRepo = new Mock<IEMailTemplateQueryRepository>();
            _mockContactRepo = new Mock<IContactQueryRepository>();
            _mockStandardService = new Mock<IStandardService>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<SendAddStandardEmailHandler>>();

            _handler = new SendAddStandardEmailHandler(_mockEmailTemplateRepo.Object, _mockContactRepo.Object, _mockStandardService.Object, _mockMediator.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_ShouldSendEmail_WhenContactAndStandardAndTemplateExists()
        {
            // Arrange
            var request = new SendAddStandardEmailRequest
            {
                ContactId = Guid.NewGuid().ToString(),
                StandardReference = "ST0001",
                StandardVersions = new List<string> { "1.0", "1.1", "1.2" }
            };

            var contact = new Contact { Email = "test@example.com", DisplayName = "Test User" };
            var standard = new Standard { Title = "Test Standard" };
            var template = new EmailTemplateSummary { TemplateName = EmailTemplateNames.EPAOStandardAdd };

            _mockContactRepo.Setup(repo => repo.GetContactById(It.IsAny<Guid>())).ReturnsAsync(contact);
            _mockEmailTemplateRepo.Setup(repo => repo.GetEmailTemplate(EmailTemplateNames.EPAOStandardAdd)).ReturnsAsync(template);
            _mockStandardService.Setup(service => service.GetStandardVersionsByIFateReferenceNumber(It.IsAny<string>())).ReturnsAsync(new List<Standard> { standard });
            
            // Act
            await _handler.Handle(request, new CancellationToken());

            // Assert
            _mockMediator.Verify(med => med.Send(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_ShouldThrowException_WhenContactDoesNotExist()
        {
            // Arrange
            var request = new SendAddStandardEmailRequest { ContactId = Guid.NewGuid().ToString() };

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _handler.Handle(request, new CancellationToken()));
        }

        [Test]
        public void Handle_ShouldThrowException_WhenStandardDoesNotExist()
        {
            // Arrange
            var request = new SendAddStandardEmailRequest { ContactId = Guid.NewGuid().ToString(), StandardReference = "ST0001" };
            var contact = new Contact { Email = "test@example.com", DisplayName = "Test User" };

            _mockContactRepo.Setup(repo => repo.GetContactById(It.IsAny<Guid>())).ReturnsAsync(contact);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _handler.Handle(request, new CancellationToken()));
        }
    }
}