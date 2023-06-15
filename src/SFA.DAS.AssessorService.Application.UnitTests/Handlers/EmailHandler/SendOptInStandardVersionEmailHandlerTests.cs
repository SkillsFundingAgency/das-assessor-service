using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.EmailHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.EmailHandler
{
    [TestFixture]
    public class SendOptInStandardVersionEmailHandlerTests
    {
        private Mock<IEMailTemplateQueryRepository> _emailTemplateQueryRepositoryMock;
        private Mock<IContactQueryRepository> _contactQueryRepositoryMock;
        private Mock<IStandardService> _standardServiceMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<SendOptInStandardVersionEmailHandler>> _loggerMock;
        private SendOptInStandardVersionEmailHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _emailTemplateQueryRepositoryMock = new Mock<IEMailTemplateQueryRepository>();
            _contactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            _standardServiceMock = new Mock<IStandardService>();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<SendOptInStandardVersionEmailHandler>>();

            _handler = new SendOptInStandardVersionEmailHandler(
                _emailTemplateQueryRepositoryMock.Object,
                _contactQueryRepositoryMock.Object,
                _standardServiceMock.Object,
                _mediatorMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldSendEmail_WhenAllRequirementsAreMet()
        {
            // Arrange
            var request = new SendOptInStandardVersionEmailRequest { ContactId = Guid.NewGuid(), StandardReference = "ST0001", Version = "1.0" };
            var contact = new Contact { Id = request.ContactId, DisplayName = "Contact", Email = "contact@example.com" };
            var standardVersions = new List<Standard> { new Standard { Title = "Title", IfateReferenceNumber = request.StandardReference } };
            var emailTemplateSummary = new EmailTemplateSummary { TemplateName = EmailTemplateNames.EPAOStandardConfimOptIn };

            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId)).ReturnsAsync(contact);
            _standardServiceMock.Setup(x => x.GetStandardVersionsByIFateReferenceNumber(request.StandardReference)).ReturnsAsync(standardVersions);
            _emailTemplateQueryRepositoryMock.Setup(x => x.GetEmailTemplate(EmailTemplateNames.EPAOStandardConfimOptIn)).ReturnsAsync(emailTemplateSummary);

            // this callback is being used to capture the send email request containing dynamic tokens
            // which cannot be verified by Moq
            var sendEmailRequestSerialized = string.Empty;
            _mediatorMock.Setup(c => c.Send(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Unit())
                .Callback<IRequest<Unit>, CancellationToken>((sendEmailRequestCallback, cancellationToken) =>
                {
                    if (sendEmailRequestCallback is SendEmailRequest sendEmailRequest)
                    {
                        sendEmailRequestSerialized = JsonConvert.SerializeObject(new
                        {
                            sendEmailRequest.Email,
                            EmailTemplate = sendEmailRequest.EmailTemplateSummary.TemplateName,
                            sendEmailRequest.Tokens
                        });
                    }
                });

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            sendEmailRequestSerialized.Should().Be(
                GetSerializedSendEmailRequest(
                    contact.Email,
                    emailTemplateSummary.TemplateName,
                    standardVersions.First().Title,
                    request.StandardReference,
                    request.Version,
                    contact.DisplayName));
        }

        [Test]
        public async Task Handle_ThrowsException_WhenContactNotFound()
        {
            // Arrange
            var request = new SendOptInStandardVersionEmailRequest { ContactId = Guid.NewGuid(), StandardReference = "ST0001", Version = "1.0" };
            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId)).ReturnsAsync((Contact)null);

            // Act
            Func<Task> func = () => _handler.Handle(request, CancellationToken.None);
            
            // Assert
            await func.Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public async Task Handle_ThrowsException_WhenNoStandardVersions()
        {
            // Arrange
            var request = new SendOptInStandardVersionEmailRequest { ContactId = Guid.NewGuid(), StandardReference = "ST0001", Version = "1.0" };
            var contact = new Contact { Id = request.ContactId, DisplayName = "Contact", Email = "contact@example.com" };
            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId)).ReturnsAsync(contact);
            _standardServiceMock.Setup(x => x.GetStandardVersionsByIFateReferenceNumber(request.StandardReference)).ReturnsAsync(new List<Standard>());

            // Act
            Func<Task> func = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public async Task Handle_ThrowsException_WhenEmailTemplateNotFound()
        {
            // Arrange
            var request = new SendOptInStandardVersionEmailRequest { ContactId = Guid.NewGuid(), StandardReference = "ST0001", Version = "1.0" };
            var contact = new Contact { Id = request.ContactId, DisplayName = "Contact", Email = "contact@example.com" };
            var standardVersions = new List<Standard> { new Standard { Title = "Title", IfateReferenceNumber = request.StandardReference } };
            _contactQueryRepositoryMock.Setup(x => x.GetContactById(request.ContactId)).ReturnsAsync(contact);
            _standardServiceMock.Setup(x => x.GetStandardVersionsByIFateReferenceNumber(request.StandardReference)).ReturnsAsync(standardVersions);
            _emailTemplateQueryRepositoryMock.Setup(x => x.GetEmailTemplate(EmailTemplateNames.EPAOStandardConfimOptIn)).ReturnsAsync((EmailTemplateSummary)null);

            // Act
            Func<Task> func = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            await func.Should().ThrowAsync<NotFoundException>();
        }

        private string GetSerializedSendEmailRequest(string email, string templateName, string standard, string standardReference, string version, string contactName)
        {
            var sendEmailRequest = JsonConvert.SerializeObject(new
            {
                Email = email,
                EmailTemplate = templateName,
                Tokens = new
                {
                    StandardReference = standardReference,
                    Standard = standard,
                    Version = version,
                    ContactName = contactName,
                    EmailTemplateTokens.ServiceName,
                    EmailTemplateTokens.ServiceTeam
                }
            });

            return sendEmailRequest;
        }
    }
}

