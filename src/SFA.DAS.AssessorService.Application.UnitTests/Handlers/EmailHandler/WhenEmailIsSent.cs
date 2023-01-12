using System;
using NUnit.Framework;
using System.Threading;
using FizzWare.NBuilder;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.Notifications.Api.Client;
using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Internal;
using SFA.DAS.AssessorService.Application.Handlers.EmailHandlers;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.EmailHandler
{
    public class WhenEmailIsSent
    {
        private Mock<INotificationsApi> _notificationApiMock;
        private SendEmailHandler _sendEmailHandler;
        private SendEmailRequest _message;
        private Mock<ILogger<SendEmailHandler>> _loggerMock;        

       [SetUp]
        public void SetUp()
        {
            _notificationApiMock = new Mock<INotificationsApi>();
             _loggerMock = new Mock<ILogger<SendEmailHandler>>();
        }
        
        [Test]
        public void Then_Should_Have_Invoked_NotificationApi_Successfully()
        {
            //arrange
            var eailTemplate = Builder<EmailTemplateSummary>.CreateNew().Build();
            eailTemplate.TemplateId = "TemplateId"; 
                
            _message = Builder<SendEmailRequest>.CreateNew().WithConstructor(() =>
                new SendEmailRequest("test@test.com", eailTemplate, new { key = "value" })).Build();            
            
            _sendEmailHandler = new SendEmailHandler(_notificationApiMock.Object, _loggerMock.Object);

            //act
            _sendEmailHandler.Handle(_message, new CancellationToken()).Wait();

            //assert
            _notificationApiMock.Verify();
        }


        [Test]
        public void Then_Should_Have_Invoked_NotificationApi_Successfully_With_No_Personalisation_Tokens()
        {
            //arrange
            var emailTemplate = Builder<EmailTemplateSummary>.CreateNew().Build();
            emailTemplate.TemplateId = "TemplateId"; 
            
            _message = Builder<SendEmailRequest>.CreateNew().WithConstructor(() =>
                new SendEmailRequest("test@test.com", emailTemplate, new {})).Build();            

            _sendEmailHandler = new SendEmailHandler(_notificationApiMock.Object, _loggerMock.Object);
            
            //act
            _sendEmailHandler.Handle(_message, new CancellationToken()).Wait();

            //assert
            _notificationApiMock.Verify();
        }

        [Test]
        public void Then_Should_Have_Invoked_NotificationApi_Successfully_With_EmailTemplateSummary_Recipients()
        {
            //arrange
            var emailTemplate = Builder<EmailTemplateSummary>.CreateNew().Build();
            emailTemplate.TemplateId = "TemplateId";
            emailTemplate.Recipients = "test@test.com";

            _message = Builder<SendEmailRequest>.CreateNew().WithConstructor(() =>
                new SendEmailRequest(string.Empty, emailTemplate,  new { })).Build();

            _sendEmailHandler = new SendEmailHandler(_notificationApiMock.Object,  _loggerMock.Object);
            
            //act
            _sendEmailHandler.Handle(_message, new CancellationToken()).Wait();

            //assert
            _notificationApiMock.Verify();
        }

        [Test]
        public void Then_Should_Have_Invoked_NotificationApi_Successfully_With_Email_And_EmailTemplateSummary_Recipients()
        {
            //arrange
            var emailTemplate = Builder<EmailTemplateSummary>.CreateNew().Build();
            emailTemplate.TemplateId = "TemplateId";
            emailTemplate.Recipients = "test@test.com";

            _message = Builder<SendEmailRequest>.CreateNew().WithConstructor(() =>
                new SendEmailRequest("testemail@test.com", emailTemplate, new { })).Build();

            _sendEmailHandler = new SendEmailHandler(_notificationApiMock.Object, _loggerMock.Object);           

            //act
            _sendEmailHandler.Handle(_message, new CancellationToken()).Wait();

            //assert
            _notificationApiMock.Verify();
        }

        [Test]
        public void Then_Fails_Due_To_Invalid_Email()
        {
            //arrange
            var emailTemplate = Builder<EmailTemplateSummary>.CreateNew().Build();
            emailTemplate.TemplateId = "TemplateId";
            emailTemplate.Recipients = string.Empty;

            _message = Builder<SendEmailRequest>.CreateNew().WithConstructor(() =>
                  new SendEmailRequest(string.Empty, emailTemplate, new { })).Build();

            _sendEmailHandler = new SendEmailHandler(_notificationApiMock.Object, _loggerMock.Object);

            //act
            _sendEmailHandler.Handle(_message, new CancellationToken()).Wait();

            //assert
            VerifyLogger(LogLevel.Error, new EventId(0));
        }

        private void VerifyLogger(LogLevel logLevel, EventId eventId)
        {
            _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(p => p == logLevel),
                It.Is<EventId>(p => p == eventId),
                It.Is<It.IsAnyType>((@object, @type) => @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
