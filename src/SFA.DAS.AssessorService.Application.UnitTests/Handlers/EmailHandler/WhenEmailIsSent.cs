using System;
using NUnit.Framework;
using System.Threading;
using FizzWare.NBuilder;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Notifications.Api.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using SFA.DAS.AssessorService.Application.Handlers.EmailHandlers;

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
            _message = Builder<SendEmailRequest>.CreateNew().WithConstructor(() =>
                new SendEmailRequest("test@test.com", 
                    Builder<EMailTemplate>.CreateNew().Build(),
                    new { key = "value" })).Build();

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
            _message = Builder<SendEmailRequest>.CreateNew().WithConstructor(() =>
                new SendEmailRequest("test@test.com",
                    Builder<EMailTemplate>.CreateNew().Build(),
                    new {})).Build();
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
            _message = Builder<SendEmailRequest>.CreateNew().WithConstructor(() =>
                new SendEmailRequest("",
                    Builder<EMailTemplate>.CreateNew().Build(),
                    new { })).Build();
            _sendEmailHandler = new SendEmailHandler(_notificationApiMock.Object, _loggerMock.Object);

            //act
            _sendEmailHandler.Handle(_message, new CancellationToken()).Wait();

            //assert
            _loggerMock.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }
    }
}
