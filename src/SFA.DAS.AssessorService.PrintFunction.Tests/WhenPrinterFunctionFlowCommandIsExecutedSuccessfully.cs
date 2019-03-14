using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.EpaoImporter;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;
using SFA.DAS.AssessorService.EpaoImporter.Sftp;

namespace SFA.DAS.AssessorService.PrintFunction.Tests
{
    public class WhenSystemSchedulesDate
    {
        private PrintProcessCommand _printProcessCommand;
        private Mock<IAggregateLogger> _aggregateLogger;
        private Mock<IPrintingSpreadsheetCreator> _printingSpreadsheetCreator;
        private Mock<IAssessorServiceApi> _assessorServiceApi;
        private Mock<INotificationService> _notificationService;
        private Mock<IFileTransferClient> _fileTransferClient;
        private Mock<IPrintingJsonCreator> _printingJsonCreator;
        [SetUp]
        public void Arrange()
        {
            _aggregateLogger = new Mock<IAggregateLogger>();
            _printingJsonCreator = new Mock<IPrintingJsonCreator>();
            _printingSpreadsheetCreator = new Mock<IPrintingSpreadsheetCreator>();
            _assessorServiceApi = new Mock<IAssessorServiceApi>();
            _notificationService = new Mock<INotificationService>();
            _fileTransferClient = new Mock<IFileTransferClient>();

            _printProcessCommand = new PrintProcessCommand(
                _aggregateLogger.Object,
                _printingJsonCreator.Object,
                _printingSpreadsheetCreator.Object,
                _assessorServiceApi.Object,
                _notificationService.Object,
                _fileTransferClient.Object
                );

            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(10).Build();

            foreach (var certificateResponse in certificateResponses)
            {
                certificateResponse.CertificateData = Builder<CertificateDataResponse>.CreateNew().Build();
            }

            _aggregateLogger.Setup(q => q.LogInfo(Moq.It.IsAny<string>()));

            _assessorServiceApi.Setup(q => q.GetCertificatesToBePrinted()
            ).Returns(Task.FromResult(certificateResponses.AsEnumerable()));

            _assessorServiceApi.Setup(q => q.GetCurrentBatchLog())
                .Returns(Task.FromResult(new BatchLogResponse
                {
                    BatchNumber = 12,
                    NumberOfCoverLetters = 12,
                    FileUploadEndTime = DateTime.Now,
                    BatchCreated = DateTime.Now,
                    CertificatesFileName = "XXXX",
                    Period = "0818",
                    FileUploadStartTime = DateTime.Now,
                    NumberOfCertificates = 12
                }));

            _assessorServiceApi.Setup(api => api.GetSchedule(ScheduleType.PrintRun))
                .ReturnsAsync(new ScheduleRun() { Id = Guid.NewGuid() });

            _fileTransferClient.Setup(client => client.GetListOfDownloadedFiles())
                .ReturnsAsync(new List<string>());

            _printProcessCommand.Execute().GetAwaiter().GetResult();
        }

        [Test]
        public void ItShouldGenerateABatchNumber()
        {
            _assessorServiceApi.Verify(q => q.GetCurrentBatchLog(), Times.Once());
        }

        [Test]
        public void ItShouldCreateIFASpreadsheet()
        {
            _printingSpreadsheetCreator.Verify(q =>
                q.Create(It.IsAny<int>(), It.IsAny<List<CertificateResponse>>()), Times.Once());
        }

        [Test]
        public void ItShouldSendANotification()
        {
            _notificationService.Verify(q =>
                q.Send(It.IsAny<int>(), It.IsAny<List<CertificateResponse>>(), It.IsAny<string>()), Times.Once());
        }


        [Test]
        public void ItShouldSetStatusOnCertificates()
        {
            _assessorServiceApi.Verify(q =>
                q.ChangeStatusToPrinted(It.IsAny<int>(), It.IsAny<List<CertificateResponse>>()), Times.Once());
        }
    }
}
