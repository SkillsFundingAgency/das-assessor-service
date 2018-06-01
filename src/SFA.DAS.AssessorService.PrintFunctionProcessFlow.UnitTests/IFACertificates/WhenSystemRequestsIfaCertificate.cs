using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.DomainServices;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Sftp;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests.IFACertificates
{
    public class WhenSystemRequestsIfaCertificate
    {
        private int _result;

        private readonly Mock<IAggregateLogger> _aggregateLoggerMock = new Mock<IAggregateLogger>();
        private readonly Mock<IFileTransferClient> _fileTransferClientMock = new Mock<IFileTransferClient>();
        private readonly Mock<IWebConfiguration> _webConfigurationMock = new Mock<IWebConfiguration>();

        [SetUp]
        public void Arrange()
        {
            _fileTransferClientMock.Setup(q => q.Send(It.IsAny<MemoryStream>(), It.IsAny<string>()));

            var ifaCertificateService = new IFACertificateService(_aggregateLoggerMock.Object,
                _fileTransferClientMock.Object,
                _webConfigurationMock.Object);

            var certificateData = Builder<CertificateDataResponse>.CreateNew().Build();
            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(1).All().With(x => x.CertificateData = certificateData).Build();
            var coverLettersProduced = new CoverLettersProduced();

            foreach (var certificateResponse in certificateResponses)
            {

            }

            ifaCertificateService.Create(1, certificateResponses, coverLettersProduced).GetAwaiter().GetResult();

        }

        [Test]
        public void ThenItShouldUpdateCertificates()
        {

        }
    }
}
