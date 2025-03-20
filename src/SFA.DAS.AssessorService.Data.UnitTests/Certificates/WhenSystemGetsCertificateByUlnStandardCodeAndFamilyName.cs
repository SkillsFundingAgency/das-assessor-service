using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.TestHelper;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemGetsCertificateByUlnStandardCodeAndFamilyName
    {
        private CertificateRepository _certificateRepository;

        [SetUp]
        public void Arrange()
        {
            var mockDbContext = CreateMockDbContext();
            var mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            mockAssessorUnitOfWork
                .SetupGet(x => x.AssessorDbContext)
                .Returns(mockDbContext.Object);

            _certificateRepository = new CertificateRepository(mockAssessorUnitOfWork.Object);
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

        private Mock<IAssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<IAssessorDbContext>();

            var certificates = Builder<Certificate>.CreateListOfSize(3)
                .TheFirst(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 1111111111)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.StandardCode = 100)
                .With(x => x.CertificateData = GetCertificateData("Mirkwood"))
                .WithPrivate(x => x.LearnerFamilyName, "Mirkwood")
                .TheNext(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 2222222222)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0002")
                .With(x => x.StandardCode = 123)
                .With(x => x.CertificateData = GetCertificateData("Hawkins"))
                .WithPrivate(x => x.LearnerFamilyName, "Hawkins")
                .TheLast(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 3333333333)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0003")
                .With(x => x.StandardCode = 232)
                .With(x => x.CertificateData = GetCertificateData("Cornwallis"))
                .WithPrivate(x => x.LearnerFamilyName, "Cornwallis")
                .Build()
                .AsQueryable();

            mockDbContext.Setup(c => c.StandardCertificates).ReturnsDbSet(certificates);

            return mockDbContext;
        }

        private CertificateData GetCertificateData(string learnerFamilyName)
        {
            var certData = new CertificateData
            {
                LearnerFamilyName = learnerFamilyName
            };
            return certData;
        }
    }
}
