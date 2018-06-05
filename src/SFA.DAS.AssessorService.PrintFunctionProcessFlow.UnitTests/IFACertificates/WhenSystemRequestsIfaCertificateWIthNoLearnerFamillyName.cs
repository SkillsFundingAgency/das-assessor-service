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
    public class WhenSystemRequestsIfaCertificateWithNoLearnerFamillyName
    {
        private int _result;

        private readonly Mock<IAggregateLogger> _aggregateLoggerMock = new Mock<IAggregateLogger>();
        private readonly Mock<IFileTransferClient> _fileTransferClientMock = new Mock<IFileTransferClient>();
        private readonly Mock<IWebConfiguration> _webConfigurationMock = new Mock<IWebConfiguration>();

        [SetUp]
        public void Arrange()
        {          
            _webConfigurationMock.Setup(foo => foo.CertificateDetails).Returns(new CertificateDetails
            {
                ChairName = "XXXXX",
                ChairTitle = "XXXX"
            });
            
            var ifaCertificateService = new IFACertificateService(_aggregateLoggerMock.Object,
                _fileTransferClientMock.Object,
                _webConfigurationMock.Object);

            var certificateData = Builder<CertificateDataResponse>.CreateNew()
                .With(q => q.LearnerGivenNames = null)
                .With(q => q.LearnerFamilyName = null)
                .Build();
            var certificateResponses = Builder<CertificateResponse>.CreateListOfSize(1).All().With(x => x.CertificateData = certificateData).Build();           

            var coverLetterProduced = new CoverLettersProduced();
            foreach (var certificateResponse in certificateResponses)
            {
                coverLetterProduced.CoverLetterFileNames.Add(certificateResponse.CertificateReference);
                coverLetterProduced.CoverLetterCertificates.Add(certificateResponse.CertificateReference, "XXXXX");
            }

            ifaCertificateService.Create(1, certificateResponses, coverLetterProduced).GetAwaiter().GetResult();

        }

        [Test]
        public void ThenItShouldUpdateCertificates()
        {

        }
    }
}
