using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemDeleteCertificate
    {
        private CertificateRepository _certificateRepository;        
        private Mock<AssessorDbContext> _mockDbContext;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Exception _exception;        
        private Guid _certificateId;
        private string _incidentNumber;
        private string _reasonForChange;

        [SetUp]
        public void Arrange()
        {
            _certificateId = Guid.NewGuid();
            _incidentNumber = "INC12345";
            _reasonForChange = "Test Text Reason For Change";

            var mockCertificate = MockDbSetCreateCertificate();
            var mockCertificateLog = MockDbSetCreateCertificateLog();
            
            _mockDbContext = CreateMockDbContext(mockCertificate, mockCertificateLog);
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            
            _certificateRepository = new CertificateRepository(_mockUnitOfWork.Object, _mockDbContext.Object);
        }


        [Test]
        public async Task Then_Delete_With_ReasonForChange()
        {
            //Act           
            await _certificateRepository.Delete(1111111111, 93, "UserName", CertificateActions.Delete, reasonForChange: _reasonForChange);
           
            //Assert
            var result = _certificateRepository.GetCertificateLogsFor(_certificateId);
            Assert.AreEqual(_reasonForChange, result.Result.First().ReasonForChange);
            Assert.AreEqual(2, result.Result.Count());
        }

        [Test]
        public async Task Then_Delete_With_IncidentNumber()
        {
            //Act
            await _certificateRepository.Delete(1111111111, 93, "UserName", CertificateActions.Delete, incidentNumber: _incidentNumber);

            //Assert
            var certificate =  _certificateRepository.GetCertificate(_certificateId);            
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.Result.CertificateData);
            Assert.AreEqual(certificateData.IncidentNumber, _incidentNumber);
        }

        private Mock<DbSet<Certificate>> MockDbSetCreateCertificate()
        {
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

            var mockCertificate = new Mock<DbSet<Certificate>>();

            mockCertificate.As<IQueryable<Certificate>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Certificate>(certificates.Provider));

            mockCertificate.As<IQueryable<Certificate>>()
                .Setup(m => m.Expression)
                .Returns(certificates.Expression);

            mockCertificate.As<IQueryable<Certificate>>()
                .Setup(m => m.ElementType)
                .Returns(certificates.ElementType);

            mockCertificate.As<IQueryable<Certificate>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => certificates.GetEnumerator());

            return mockCertificate;
        }

        private Mock<DbSet<CertificateLog>> MockDbSetCreateCertificateLog()
        {
            var certificateLogs = Builder<CertificateLog>.CreateListOfSize(1)
                .TheFirst(1)
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.CertificateId = _certificateId)
                .With(x => x.Action = CertificateActions.Delete)
                .With(x => x.EventTime = DateTime.UtcNow)
                .With(x => x.ReasonForChange = "Test")
                .Build();                

            var mockCertificateLog = new Mock<DbSet<CertificateLog>>();

            mockCertificateLog.As<IQueryable<CertificateLog>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<CertificateLog>(certificateLogs.AsQueryable().Provider));

            mockCertificateLog.As<IQueryable<CertificateLog>>()
                .Setup(m => m.Expression)
                .Returns(certificateLogs.AsQueryable().Expression);

            mockCertificateLog.As<IQueryable<CertificateLog>>()
                .Setup(m => m.ElementType)
                .Returns(certificateLogs.AsQueryable().ElementType);


            mockCertificateLog.As<IQueryable<CertificateLog>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => certificateLogs.GetEnumerator());

            mockCertificateLog
               .Setup(m => m.AddAsync(It.IsAny<CertificateLog>(), It.IsAny<CancellationToken>()))
               .Callback((CertificateLog entity, CancellationToken token) => { certificateLogs.Add(entity); });
            //.Returns((CertificateLog entity, CancellationToken token) => Task.FromResult((EntityEntry<CertificateLog>)null));      // @ToDo: .Net Core 3.1 upgrade - uncomment and fix this

            return mockCertificateLog;
        }

        private Mock<AssessorDbContext> CreateMockDbContext(Mock<DbSet<Certificate>> certificateMockDbSet, Mock<DbSet<CertificateLog>> CreateCertificateLogMockDbSet)
        {
            var mockDbContext = new Mock<AssessorDbContext>();
            mockDbContext.Setup(c => c.Certificates).Returns(certificateMockDbSet.Object);
            mockDbContext.Setup(c => c.CertificateLogs).Returns(CreateCertificateLogMockDbSet.Object);
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
