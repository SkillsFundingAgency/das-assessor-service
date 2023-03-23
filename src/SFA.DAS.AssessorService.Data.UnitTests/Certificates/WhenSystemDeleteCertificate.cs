using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Moq.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemDeleteCertificate
    {
        private CertificateRepository _certificateRepository;
        private Mock<AssessorDbContext> _mockDbContext;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Guid _certificateId;
        private string _incidentNumber;
        private string _reasonForChange;

        [SetUp]
        public void Arrange()
        {
            _certificateId = Guid.NewGuid();
            _incidentNumber = "INC12345";
            _reasonForChange = "Test Text Reason For Change";

            _mockDbContext = CreateMockDbContext();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            
            _certificateRepository = new CertificateRepository(_mockUnitOfWork.Object, _mockDbContext.Object);
        }

        [Test]
        public async Task Then_Delete_With_ReasonForChange()
        {
            // Act
            await _certificateRepository.Delete(1111111111, 93, "UserName", CertificateActions.Delete, reasonForChange: _reasonForChange);
           
            // Assert
            var result = _certificateRepository.GetCertificateLogsFor(_certificateId);
            Assert.AreEqual(_reasonForChange, result.Result.First().ReasonForChange);
            Assert.AreEqual(2, result.Result.Count());
        }

        [Test]
        public async Task Then_Delete_With_IncidentNumber()
        {
            // Act
            await _certificateRepository.Delete(1111111111, 93, "UserName", CertificateActions.Delete, incidentNumber: _incidentNumber);

            // Assert
            var certificate =  _certificateRepository.GetCertificate(_certificateId);            
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.Result.CertificateData);
            Assert.AreEqual(certificateData.IncidentNumber, _incidentNumber);
        }

        private Mock<AssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<AssessorDbContext>();

            var certificates = Builder<Certificate>.CreateListOfSize(2)
                .TheFirst(1)
                .With(x => x.Id = _certificateId)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.CertificateData = GetCertificateData())
                .With(x => x.Uln = 1111111111)
                .With(x => x.StandardCode = 93)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.IsPrivatelyFunded = true)
                .TheNext(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 1111111111)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.StandardCode = 100)
                .With(x => x.IsPrivatelyFunded = true)
                .Build()
                .AsQueryable();

            mockDbContext.Setup(x => x.Certificates).ReturnsDbSet(certificates);

            var certificateLogs = Builder<CertificateLog>.CreateListOfSize(1)
                .TheFirst(1)
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.CertificateId = _certificateId)
                .With(x => x.Action = CertificateActions.Delete)
                .With(x => x.EventTime = DateTime.UtcNow)
                .With(x => x.ReasonForChange = "Test")
                .Build();

            var mockCertificateLog = new Mock<DbSet<CertificateLog>>();
            mockCertificateLog
                .Setup(m => m.Add(It.IsAny<CertificateLog>()))
                .Callback((CertificateLog entity) => certificateLogs.Add(entity))
                .Returns((EntityEntry<CertificateLog>)null);

            mockDbContext.Setup(x => x.CertificateLogs).ReturnsDbSet(certificateLogs, mockCertificateLog);
            
            return mockDbContext;
        }

        private string GetCertificateData()
        {
            var certData = new CertificateData
            {
                ContactName = "ContactName"
            };
            return JsonConvert.SerializeObject(certData);
        }
    }
}
