using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemsGetsCertificateByUlnAndLastname
    {
        private CertificateRepository _certificateRepository;
        private Mock<AssessorDbContext> _mockDbContext;
        private Mock<IDbConnection> _mockDbConnection;
        private Certificate _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();
            
            var mockSet = CreateCertificateMockDbSet();
            _mockDbContext = CreateMockDbContext(mockSet);

            _mockDbConnection = new Mock<IDbConnection>();

            _certificateRepository = new CertificateRepository(_mockDbContext.Object,
                _mockDbConnection.Object);
            _certificateRepository = new CertificateRepository(_mockDbContext.Object, new Mock<IDbConnection>().Object);

            _result = _certificateRepository.GetCertificateByUlnLastname(1111111111, "Hawkins").Result;
        }

        [Test]
        public void ItShouldReturnResult()
        {
            _result.Uln.Should().Be(1111111111);
        }

        private Mock<DbSet<Certificate>> CreateCertificateMockDbSet()
        {
            var certificates = Builder<Certificate>.CreateListOfSize(10)
                .TheFirst(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 1111111111)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.CertificateData = "{'LearnerFamilyName':'Hawkins'}")
                .TheNext(9)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 1111111111)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.CertificateData = "{'LearnerFamilyName':'Hawkins'}")
                .Build()
                .AsQueryable();

            var mockSet = new Mock<DbSet<Certificate>>();

            mockSet.As<IAsyncEnumerable<Certificate>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestAsyncEnumerator<Certificate>(certificates.GetEnumerator()));

            mockSet.As<IQueryable<Certificate>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Certificate>(certificates.Provider));

            mockSet.As<IQueryable<Certificate>>().Setup(m => m.Expression).Returns(certificates.Expression);
            mockSet.As<IQueryable<Certificate>>().Setup(m => m.ElementType).Returns(certificates.ElementType);
            mockSet.As<IQueryable<Certificate>>().Setup(m => m.GetEnumerator()).Returns(() => certificates.GetEnumerator());

            return mockSet;
        }

        private Mock<AssessorDbContext> CreateMockDbContext(Mock<DbSet<Certificate>> certificateMockDbSet)
        {
            var mockDbContext = new Mock<AssessorDbContext>();
            mockDbContext.Setup(c => c.Certificates).Returns(certificateMockDbSet.Object);
            return mockDbContext;
        }
    }
}
