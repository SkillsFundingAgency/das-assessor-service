using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.TestHelper;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Linq;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemsGetsCertificateByUlnAndFamilyName
    {
        private CertificateRepository _certificateRepository;
        private Mock<AssessorDbContext> _mockDbContext;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Certificate _result;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();
            
            _mockDbContext = CreateMockDbContext();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _certificateRepository = new CertificateRepository(_mockUnitOfWork.Object, _mockDbContext.Object);

            _result = _certificateRepository.GetCertificate(22222222222, "Hawkins").Result;
        }

        [Test]
        public void ItShouldReturnResult()
        {
            _result.Uln.Should().Be(22222222222);
            _result.LearnerFamilyName.Should().Be("Hawkins");
        }

        private Mock<AssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<AssessorDbContext>();

            var certificates = Builder<Certificate>.CreateListOfSize(10)
                .TheFirst(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 1111111111)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.CertificateData = "{'LearnerFamilyName':'Mirkwood'}")
                .WithPrivate(x => x.LearnerFamilyName, "Mirkwood")
                .TheNext(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 22222222222)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0002")
                .With(x => x.CertificateData = "{'LearnerFamilyName':'Hawkins'}")
                .WithPrivate(x => x.LearnerFamilyName, "Hawkins")
                .TheNext(8)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 33333333333)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0003")
                .With(x => x.CertificateData = "{'LearnerFamilyName':'Cornwallis'}")
                .WithPrivate(x => x.LearnerFamilyName, "Cornwallis")
                .Build()
                .AsQueryable();

            mockDbContext.Setup(c => c.Certificates).ReturnsDbSet(certificates);

            return mockDbContext;
        }
    }
}
