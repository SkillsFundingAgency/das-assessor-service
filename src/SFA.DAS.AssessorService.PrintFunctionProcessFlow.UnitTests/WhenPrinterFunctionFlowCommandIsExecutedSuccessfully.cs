using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.EpaoImporter;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Notification;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests
{
    public class WhenPrinterFunctionFlowCommandIsExecutedSuccessfully
    {
        private PrintProcessFlowCommand _printProcessFlowCommand;
        private Mock<IAggregateLogger> _aggregateLogger;
        private Mock<ICoverLetterService> _coverLetterService;
        private Mock<IIFACertificateService> _ifaCertificateService;
        private Mock<IAssessorServiceApi> _assessorServiceApi;
        private Mock<INotificationService> _notificationService;

        [SetUp]
        public void Arrange()
        {
            _aggregateLogger = new Mock<IAggregateLogger>();
            _coverLetterService = new Mock<ICoverLetterService>();
            _ifaCertificateService = new Mock<IIFACertificateService>();
            _assessorServiceApi= new Mock<IAssessorServiceApi>();
            _notificationService= new Mock<INotificationService>();


            _printProcessFlowCommand = new PrintProcessFlowCommand(
                _aggregateLogger.Object,
                _coverLetterService.Object,
                _ifaCertificateService.Object,
                _assessorServiceApi.Object,
                _notificationService.Object
                );

            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(10).Build();
            _aggregateLogger.Setup(q => q.LogInfo(Moq.It.IsAny<string>()));
            _assessorServiceApi.Setup(q => q.GetCertificatesToBePrinted()
            ).Returns(Task.FromResult(certificateResponses.AsEnumerable()));          

            _printProcessFlowCommand.Execute();
        }

        [Test]
        public void ItShouldGenerateABatchNumber()
        {
            _assessorServiceApi.Verify(q => q.GenerateBatchNumber(), Times.Once());
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
