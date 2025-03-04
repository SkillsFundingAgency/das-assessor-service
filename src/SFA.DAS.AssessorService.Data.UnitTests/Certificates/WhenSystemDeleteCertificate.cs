using System;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemDeleteCertificate
    {
        private CertificateRepository _certificateRepository;
        private Guid _certificateId;
        private string _incidentNumber;
        private string _reasonForChange;

        [SetUp]
        public void Arrange()
        {
            _certificateId = Guid.NewGuid();
            _incidentNumber = "INC12345";
            _reasonForChange = "Test Text Reason For Change";

            var mockDbContext = CreateMockDbContext();
            var mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            mockAssessorUnitOfWork
                .SetupGet(p => p.AssessorDbContext)
                .Returns(mockDbContext.Object);

            _certificateRepository = new CertificateRepository(mockAssessorUnitOfWork.Object);
        }

        [Test]
        public async Task Then_Delete_With_ReasonForChange()
        {
            // Act
            await _certificateRepository.Delete(1111111111, 93, "UserName", CertificateActions.Delete, reasonForChange: _reasonForChange);
           
            // Assert
            var result = _certificateRepository.GetCertificateLogsFor(_certificateId);
            result.Result[0].ReasonForChange.Should().Be(_reasonForChange);
            result.Result.Count.Should().Be(2);
        }

        [Test]
        public async Task Then_Delete_With_IncidentNumber()
        {
            // Act
            await _certificateRepository.Delete(1111111111, 93, "UserName", CertificateActions.Delete, incidentNumber: _incidentNumber);

            // Assert
            var certificate =  _certificateRepository.GetCertificate<Certificate>(_certificateId);
            _incidentNumber.Should().Be(certificate.Result.CertificateData.IncidentNumber);
        }

        private Mock<IAssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<IAssessorDbContext>();

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

            mockDbContext.Setup(x => x.StandardCertificates).ReturnsDbSet(certificates);
            mockDbContext.Setup(x => x.Set<Certificate>()).ReturnsDbSet(certificates);

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

        private CertificateData GetCertificateData()
        {
            var certData = new CertificateData
            {
                ContactName = "ContactName"
            };
            return certData;
        }
    }
}
