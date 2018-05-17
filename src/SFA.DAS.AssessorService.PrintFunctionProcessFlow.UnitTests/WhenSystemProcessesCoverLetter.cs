using System.IO;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.DomainServices;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Sftp;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests
{
    public class WhenSystemProcessesCoverLetter
    {
        private int _result;
        private CoverLettersProduced _coverLettersProduced;

        [SetUp]
        public void Arrange()
        {
            var aggregateLoggerMock = new Mock<IAggregateLogger>();
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var fileLocation = directory + "\\IFATemplateDocument.docx";
            var fileStream = File.OpenRead(fileLocation);
            var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            fileStream.Close();

            var fileTransferClientMock = new Mock<IFileTransferClient>();

            var documentTemplateStreamMock = new Mock<IDocumentTemplateDataStream>();

            documentTemplateStreamMock.Setup(q => q.Get())
                .Returns(Task.FromResult(memoryStream));

            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(10).Build();
            foreach (var certificateResponse in certificateResponses)
            {
                certificateResponse.CertificateData = Builder<CertificateDataResponse>.CreateNew().Build();
            };

            var coverLetterService = new CoverLetterService(
                aggregateLoggerMock.Object,
                fileTransferClientMock.Object,
                documentTemplateStreamMock.Object
                );

            _coverLettersProduced = coverLetterService.Create(1, certificateResponses).GetAwaiter().GetResult();

        }

        [Test]
        public void ThenItShouldGenerateABatchNumber()
        {
            _coverLettersProduced.CoverLetterCertificates.Count.Should().Be(10);
        }
    }
}
