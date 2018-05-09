using System.Linq;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Notifications.Api.Client;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests
{
    public class WhenSystemSendsNotification
    {
        [SetUp]
        public void Arrange()
        {
            var aggregateLoggerMock = new Mock<IAggregateLogger>();
            var webConfigurationMock = new Mock<IWebConfiguration>();
            var assessorServiceApiMock = new Mock<IAssessorServiceApi>();
            var notificationsApi = new Mock<INotificationsApi>();

            var notificationService = new NotificationService(
                notificationsApi.Object,
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

            notificationService.Send(1, certificateResponses, coverLettersProduced).GetAwaiter().GetResult();
        }

        [Test]
        public void ThenItShouldSendNotification()
        {
        }
    }
}
