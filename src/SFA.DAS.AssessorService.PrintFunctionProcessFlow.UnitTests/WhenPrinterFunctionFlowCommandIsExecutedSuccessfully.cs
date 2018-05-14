using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.EpaoImporter;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.DomainServices;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests
{
    public class WhenSystemSchedulesDate
    {
        private PrintProcessFlowCommand _printProcessFlowCommand;
        private Mock<IAggregateLogger> _aggregateLogger;
        private Mock<ICoverLetterService> _coverLetterService;
        private Mock<IIFACertificateService> _ifaCertificateService;
        private Mock<IAssessorServiceApi> _assessorServiceApi;
        private Mock<INotificationService> _notificationService;
        private Mock<ISanitiserService> _sanitizerServiceMock;

        [SetUp]
        public void Arrange()
        {
            _aggregateLogger = new Mock<IAggregateLogger>();
            _coverLetterService = new Mock<ICoverLetterService>();
            _ifaCertificateService = new Mock<IIFACertificateService>();
            _assessorServiceApi = new Mock<IAssessorServiceApi>();
            _notificationService = new Mock<INotificationService>();
            _sanitizerServiceMock = new Mock<ISanitiserService>();


            _printProcessFlowCommand = new PrintProcessFlowCommand(
                _aggregateLogger.Object,
                _sanitizerServiceMock.Object,
                _coverLetterService.Object,
                _ifaCertificateService.Object,
                _assessorServiceApi.Object,
                _notificationService.Object
                );

            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(10).Build();

            _aggregateLogger.Setup(q => q.LogInfo(Moq.It.IsAny<string>()));

            _assessorServiceApi.Setup(q => q.GetCertificatesToBePrinted()
            ).Returns(Task.FromResult(certificateResponses.AsEnumerable()));

            _sanitizerServiceMock.Setup(q => q.Sanitise(It.IsAny<List<CertificateResponse>>())
                ).Returns(certificateResponses.ToList());

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

            _printProcessFlowCommand.Execute();
        }

        [Test]
        public void ItShouldGenerateABatchNumber()
        {
            _assessorServiceApi.Verify(q => q.GetCurrentBatchLog(), Times.Once());
        }

        [Test]
        public void ItShouldCreateACoverLetters()
        {
            _coverLetterService.Verify(q =>
                q.Create(It.IsAny<int>(), It.IsAny<IEnumerable<CertificateResponse>>()), Times.Once());
        }

        [Test]
        public void ItShouldCreateIFASpreadsheet()
        {
            _ifaCertificateService.Verify(q =>
                q.Create(It.IsAny<int>(), It.IsAny<List<CertificateResponse>>(), It.IsAny<CoverLettersProduced>()), Times.Once());
        }

        [Test]
        public void ItShouldSendANotification()
        {
            _notificationService.Verify(q =>
                q.Send(It.IsAny<int>(), It.IsAny<List<CertificateResponse>>(), It.IsAny<CoverLettersProduced>()), Times.Once());
        }


        [Test]
        public void ItShouldStausesOnCertificates()
        {
            _assessorServiceApi.Verify(q =>
                q.ChangeStatusToPrinted(It.IsAny<int>(), It.IsAny<List<CertificateResponse>>()), Times.Once());
        }
    }
}
