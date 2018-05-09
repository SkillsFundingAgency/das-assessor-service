using System;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests
{
    public class WhenSystemSendsNotification
    {
        private Mock<INotificationsApi> _notificationsApiMock;

        [SetUp]
        public void Arrange()
        {
            var aggregateLoggerMock = new Mock<IAggregateLogger>();

            var assessorServiceApiMock = new Mock<IAssessorServiceApi>();
            _notificationsApiMock = new Mock<INotificationsApi>();

            var webConfigurationMock = new Mock<IWebConfiguration>();

            var notificationService = new NotificationService(
                _notificationsApiMock.Object,
                aggregateLoggerMock.Object,
                webConfigurationMock.Object,
                assessorServiceApiMock.Object
            );

            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(10).Build().ToList();
            foreach (var certificateResponse in certificateResponses)
            {
                certificateResponse.CertificateData = Builder<CertificateDataResponse>.CreateNew().Build();
            }

            var coverLettersProduced = Builder<CoverLettersProduced>.CreateNew().Build();

            webConfigurationMock.SetupGet(q => q.Sftp)
                .Returns(new SftpSettings
                {
                    UploadDirectory = "C:\\testupoad",
                    ProofDirectory = "C:\\proofdirectory"
                });

            webConfigurationMock.SetupGet(q => q.NotificationsApiClientConfiguration)
                .Returns(new NotificationsApiClientConfiguration
                {
                    ApiBaseUrl = "http://localhost",
                    ClientToken = "12323333333333333"
                });

            assessorServiceApiMock.Setup(q => q.GetEmailTemplate())
                .Returns(Task.FromResult(new Domain.Entities.EMailTemplate
                {
                    Recipients = "jcoxhead@hotmil.com",
                    TemplateId = "123",
                    TemplateName = "ProcessMyItems",
                    Id = Guid.NewGuid()
                }));

            notificationService.Send(1, certificateResponses, coverLettersProduced).GetAwaiter().GetResult();
        }

        [Test]
        public void ThenItShouldSendNotification()
        {
            _notificationsApiMock.Verify(q => q.SendEmail(It.IsAny<Email>()), Times.Once);
        }
    }
}
