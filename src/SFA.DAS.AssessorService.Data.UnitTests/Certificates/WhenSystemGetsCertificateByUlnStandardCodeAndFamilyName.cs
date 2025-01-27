using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.TestHelper;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemGetsCertificateByUlnStandardCodeAndFamilyName
    {

        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<AssessorDbContext> _mockDbContext;
        
        private CertificateRepository _certificateRepository;

        [SetUp]
        public void Arrange()
        {
            _mockDbContext = CreateMockDbContext();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _certificateRepository = new CertificateRepository(_mockUnitOfWork.Object, _mockDbContext.Object);
        }

        [Test]
        public async Task Then_ReturnResult()
        {
            var result = await _certificateRepository.GetCertificate(2222222222, 123, "Hawkins");

            result.Uln.Should().Be(2222222222);
            result.StandardCode.Should().Be(123);
            result.LearnerFamilyName.Should().Be("Hawkins");
        }

        [Test]
        public async Task And_FamilyNameIsNotCorrect_Then_ReturnNull()
        {
            var result = await _certificateRepository.GetCertificate(2222222222, 123, "Incorrect");

            result.Should().BeNull();
        }

        [Test]
        public async Task And_UlnIsIncorrect_Then_ReturnNull()
        {
            var result = await _certificateRepository.GetCertificate(9999999999, 123, "Hawkins");

            result.Should().BeNull();
        }

        [Test]
        public async Task And_StandardCodeIsIncorrect_Then_ReturnNull()
        {
            var result = await _certificateRepository.GetCertificate(2222222222, 999, "Hawkins");

            result.Should().BeNull();
        }

        private Mock<AssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<AssessorDbContext>();

            var certificates = Builder<Certificate>.CreateListOfSize(3)
                .TheFirst(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 1111111111)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.StandardCode = 100)
                .With(x => x.CertificateData = "{'LearnerFamilyName':'Mirkwood'}")
                .WithPrivate(x => x.LearnerFamilyName, "Mirkwood")
                .TheNext(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 2222222222)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0002")
                .With(x => x.StandardCode = 123)
                .With(x => x.CertificateData = "{'LearnerFamilyName':'Hawkins'}")
                .WithPrivate(x => x.LearnerFamilyName, "Hawkins")
                .TheLast(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 3333333333)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0003")
                .With(x => x.StandardCode = 232)
                .With(x => x.CertificateData = "{'LearnerFamilyName':'Cornwallis'}")
                .WithPrivate(x => x.LearnerFamilyName, "Cornwallis")
                .Build()
                .AsQueryable();

            mockDbContext.Setup(c => c.Certificates).ReturnsDbSet(certificates);

            return mockDbContext;
        }
    }
}
