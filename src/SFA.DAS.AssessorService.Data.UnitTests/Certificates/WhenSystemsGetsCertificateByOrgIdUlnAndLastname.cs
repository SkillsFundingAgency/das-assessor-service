using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemsGetsCertificateByOrgIdUlnAndLastname
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

            _result = _certificateRepository.GetCertificateByUlnOrgIdLastnameAndStandardCode(1111111111, "EPA0001", "Hawkins", 1).Result;
        }

        [Test]
        public void ItShouldReturnResult()
        {
            _result.Uln.Should().Be(1111111111);
        }

        private Mock<AssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<AssessorDbContext>();

            var certificates = Builder<Certificate>.CreateListOfSize(10)
                .TheFirst(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 1111111111)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.CertificateData = "{'LearnerFamilyName':'Hawkins'}")
                .With(x => x.StandardCode = 1)
                .TheNext(9)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 1111111111)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.CertificateData = "{'LearnerFamilyName':'Hawkins'}")
                .Build()
                .AsQueryable();

            mockDbContext.Setup(c => c.Certificates).ReturnsDbSet(certificates);
            
            return mockDbContext;
        }
    }
}
