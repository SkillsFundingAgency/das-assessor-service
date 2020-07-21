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
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.EmailHandler
{
    public class WhenEmailIsSent
    {
        private Mock<INotificationsApi> _notificationApiMock;
        private SendEmailHandler _sendEmailHandler;
        private SendEmailRequest _message;
        private Mock<ILogger<SendEmailHandler>> _loggerMock;
        private Mock<IEMailTemplateQueryRepository> _emailTemplateQueryRepository;

       [SetUp]
        public void SetUp()
        {
            _notificationApiMock = new Mock<INotificationsApi>();
            _emailTemplateQueryRepository = new Mock<IEMailTemplateQueryRepository>();

             _loggerMock = new Mock<ILogger<SendEmailHandler>>();
        }
        
        [Test]
        public void Then_Should_Have_Invoked_NotificationApi_Successfully()
        {
            //arrange
            var firstEmailTemplate = Builder<EmailTemplateSummary>.CreateNew().Build();
            firstEmailTemplate.TemplateId = "FirstTemplateId"; 
                
            _message = Builder<SendEmailRequest>.CreateNew().WithConstructor(() =>
                new SendEmailRequest("test@test.com", firstEmailTemplate
                    ,
                    new { key = "value" })).Build();

            var secondEmailTemplate = Builder<EmailTemplateSummary>.CreateNew().Build();            
            
            _emailTemplateQueryRepository.Setup(x => x.GetEmailTemplate(firstEmailTemplate.TemplateId)).ReturnsAsync(secondEmailTemplate);
            
            _sendEmailHandler = new SendEmailHandler(_notificationApiMock.Object, _emailTemplateQueryRepository.Object,_loggerMock.Object);

            //act
            _sendEmailHandler.Handle(_message, new CancellationToken()).Wait();

            //assert
            _notificationApiMock.Verify();
        }


        [Test]
        public void Then_Should_Have_Invoked_NotificationApi_Successfully_With_No_Personalisation_Tokens()
        {
            //arrange
            var firstEmailTemplate = Builder<EmailTemplateSummary>.CreateNew().Build();
            firstEmailTemplate.TemplateId = "FirstTemplateId"; 
            
            _message = Builder<SendEmailRequest>.CreateNew().WithConstructor(() =>
                new SendEmailRequest("test@test.com",
                    firstEmailTemplate,
                    new {})).Build();

            var secondEmailTemplate = Builder<EmailTemplateSummary>.CreateNew().Build();            
            
            _emailTemplateQueryRepository.Setup(x => x.GetEmailTemplate(firstEmailTemplate.TemplateId)).ReturnsAsync(secondEmailTemplate);

            _sendEmailHandler = new SendEmailHandler(_notificationApiMock.Object, _emailTemplateQueryRepository.Object,_loggerMock.Object);
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
                    Builder<EmailTemplateSummary>.CreateNew().Build(),
                    new { })).Build();
            _emailTemplateQueryRepository.Setup(x => x.GetEmailTemplate(It.IsAny<string>())).ReturnsAsync(Builder<EmailTemplateSummary>.CreateNew().Build());
            _sendEmailHandler = new SendEmailHandler(_notificationApiMock.Object, _emailTemplateQueryRepository.Object, _loggerMock.Object);

            //act
            _sendEmailHandler.Handle(_message, new CancellationToken()).Wait();

            //assert
            _loggerMock.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }
    }
}
