using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemGetsCertificateHistory
    {
        private CertificateRepository _certificateRepository;
        private Mock<AssessorDbContext> _mockDbContext;
        private Mock<IDbConnection> _mockDbConnection;
        private Guid _organisationId;
        private Guid _certificateId;
        private PaginatedList<Certificate> _result;
        private string _userName;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            _certificateId = Guid.NewGuid();
            _userName = "epao0001";

            var mockSet = CreateCertificateMockDbSet();
            var organisationMock = CreateOrganisationMockDbSet();
            var contactMockDbSet = CreateContactMockDbSet();
            var certificateLogsMockDbSet = CreateCertificateLogsMockDbSet();

            _mockDbContext = CreateMockDbContext(mockSet, organisationMock, contactMockDbSet,
                certificateLogsMockDbSet);

            _mockDbConnection = new Mock<IDbConnection>();

            _certificateRepository = new CertificateRepository(_mockDbContext.Object,
                _mockDbConnection.Object);
            _certificateRepository = new CertificateRepository(_mockDbContext.Object, new Mock<IDbConnection>().Object);

            _result = _certificateRepository.GetCertificateHistory(_userName, 1, 10).Result;
        }

        [Test]
        public void ItShouldReturnResult()
        {
            _result.Items.Count.Should().Be(1);
        }

        private Mock<DbSet<Certificate>> CreateCertificateMockDbSet()
        {
            _organisationId = Guid.NewGuid();

            var certificates = Builder<Certificate>.CreateListOfSize(30)
                .TheFirst(1)
                .With(x => x.Id = _certificateId)
                .With(x => x.Status = CertificateStatus.Submitted)
                .With(x => x.OrganisationId = _organisationId)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew()
                    .With(q => q.Id = _organisationId)
                    .With(q => q.Contacts = Builder<Contact>.CreateListOfSize(2)
                        .All()
                        .With(a => a.OrganisationId = _organisationId)
                        .With(a => a.Username = _userName).Build())
                    .Build())
                .With(x => x.CertificateLogs = Builder<CertificateLog>.CreateListOfSize(10)
                    .All()
                    .With(q => q.CertificateId = _certificateId)
                    .Build())
                .With(x => x.Uln = 1111111111)
                .With(x => x.StandardCode = 93)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.IsPrivatelyFunded = true)
                .TheNext(29)
                .With(x => x.Status = CertificateStatus.Submitted)
                .With(x => x.OrganisationId = _organisationId)                
                .With(x => x.Organisation = Builder<Organisation>.CreateNew()                                        
                    .With(q => q.Id = _organisationId)
                    .With(q => q.Contacts = Builder<Contact>.CreateListOfSize(2)
                        .All()
                        .With(a => a.OrganisationId = _organisationId)
                        .With(a => a.Username = _userName).Build())
                    .Build())               
                .With(x => x.CertificateLogs = Builder<CertificateLog>.CreateListOfSize(10)
                    .All()
                    .With(q => q.CertificateId = x.Id)
                    .Build())
                .With(x => x.IsPrivatelyFunded = true)
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
            mockSet.As<IQueryable<Certificate>>().Setup(m => m.GetEnumerator())
                .Returns(() => certificates.GetEnumerator());

            return mockSet;
        }
        
        private Mock<DbSet<CertificateLog>> CreateCertificateLogsMockDbSet()
        {          
            var certificatesLogs = Builder<CertificateLog>.CreateListOfSize(30)
                .All()
                    .With(q => q.CertificateId = _certificateId)
                .Build()
                .AsQueryable();

            var mockSet = new Mock<DbSet<CertificateLog>>();

            mockSet.As<IAsyncEnumerable<CertificateLog>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestAsyncEnumerator<CertificateLog>(certificatesLogs.GetEnumerator()));

            mockSet.As<IQueryable<CertificateLog>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<CertificateLog>(certificatesLogs.Provider));

            mockSet.As<IQueryable<CertificateLog>>().Setup(m => m.Expression).Returns(certificatesLogs.Expression);
            mockSet.As<IQueryable<CertificateLog>>().Setup(m => m.ElementType).Returns(certificatesLogs.ElementType);
            mockSet.As<IQueryable<CertificateLog>>().Setup(m => m.GetEnumerator())
                .Returns(() => certificatesLogs.GetEnumerator());

            return mockSet;
        }

        private Mock<DbSet<Organisation>> CreateOrganisationMockDbSet()
        {
            var organisations = Builder<Organisation>.CreateListOfSize(1)
                .All()
                .With(q => q.Id = _organisationId)
                .With(q => q.Contacts = Builder<Contact>.CreateListOfSize(2)
                    .All()
                    .With(a => a.OrganisationId = _organisationId)
                    .With(a => a.Username = _userName).Build())
                .Build()
                .AsQueryable();

            var mockSet = new Mock<DbSet<Organisation>>();
            mockSet.As<IAsyncEnumerable<Organisation>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestAsyncEnumerator<Organisation>(organisations.GetEnumerator()));
            mockSet.As<IQueryable<Organisation>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Organisation>(organisations.Provider));
            mockSet.As<IQueryable<Organisation>>().Setup(m => m.Expression).Returns(organisations.Expression);
            mockSet.As<IQueryable<Organisation>>().Setup(m => m.ElementType).Returns(organisations.ElementType);
            mockSet.As<IQueryable<Organisation>>().Setup(m => m.GetEnumerator())
                .Returns(() => organisations.GetEnumerator());
            return mockSet;
        }

        private Mock<DbSet<Contact>> CreateContactMockDbSet()
        {
            var contacts = Builder<Contact>.CreateListOfSize(1)
                .All()
                .With(q => q.OrganisationId = _organisationId)
                .With(q => q.Username = _userName)
                .Build()
                .AsQueryable();

            var mockSet = new Mock<DbSet<Contact>>();
            mockSet.As<IAsyncEnumerable<Contact>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestAsyncEnumerator<Contact>(contacts.GetEnumerator()));
            mockSet.As<IQueryable<Contact>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Contact>(contacts.Provider));
            mockSet.As<IQueryable<Contact>>().Setup(m => m.Expression).Returns(contacts.Expression);
            mockSet.As<IQueryable<Contact>>().Setup(m => m.ElementType).Returns(contacts.ElementType);
            mockSet.As<IQueryable<Contact>>().Setup(m => m.GetEnumerator())
                .Returns(() => contacts.GetEnumerator());
            return mockSet;
        }

        private Mock<AssessorDbContext> CreateMockDbContext(Mock<DbSet<Certificate>> certificateMockDbSet,
            Mock<DbSet<Organisation>> organisationMockSet, Mock<DbSet<Contact>> contactModck,
            Mock<DbSet<CertificateLog>> certificateLogsMockDbSet)
        {
            var mockDbContext = new Mock<AssessorDbContext>();
            mockDbContext.Setup(c => c.Certificates).Returns(certificateMockDbSet.Object);
            mockDbContext.Setup(c => c.Organisations).Returns(organisationMockSet.Object);
            mockDbContext.Setup(c => c.Contacts).Returns(contactModck.Object);
            mockDbContext.Setup(c => c.CertificateLogs).Returns(certificateLogsMockDbSet.Object);
            return mockDbContext;
        }
    }
}