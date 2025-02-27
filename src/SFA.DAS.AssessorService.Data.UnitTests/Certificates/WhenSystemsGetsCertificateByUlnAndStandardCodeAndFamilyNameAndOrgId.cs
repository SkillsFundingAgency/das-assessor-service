using System.Linq;
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
    public class WhenSystemsGetsCertificateByUlnAndStandardCodeAndFamilyNameAndOrgId
    {
        private CertificateRepository _certificateRepository;

        [SetUp]
        public void Arrange()
        {
            var mockDbContext = CreateMockDbContext();
            var unitOfWork = new Mock<IAssessorUnitOfWork>();
            unitOfWork
                .SetupGet(x => x.AssessorDbContext)
                .Returns(mockDbContext.Object);

            _certificateRepository = new CertificateRepository(unitOfWork.Object);
        }

        [Test]
        public void ItShouldReturnResult()
        {
            var result = _certificateRepository.GetCertificate(2222222222, 2, "Hawkins", "EPA0002").Result;

            result.Uln.Should().Be(2222222222);
            result.StandardCode.Should().Be(2);
            result.LearnerFamilyName.Should().Be("Hawkins");
        }

        private Mock<AssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<AssessorDbContext>();

            var certificates = Builder<Certificate>.CreateListOfSize(10)
                .TheFirst(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 1111111111)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.CertificateData = GetCertificateData("Mirkwood"))
                .WithPrivate(x => x.LearnerFamilyName, "Mirkwood")
                .With(x => x.StandardCode = 1)
                .TheNext(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 2222222222)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0002")
                .With(x => x.CertificateData = GetCertificateData("Hawkins"))
                .WithPrivate(x => x.LearnerFamilyName, "Hawkins")
                .With(x => x.StandardCode = 2)
                .TheNext(8)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 3333333333)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0003")
                .With(x => x.CertificateData = GetCertificateData("Cornwallis"))
                .WithPrivate(x => x.LearnerFamilyName, "Cornwallis")
                .With(x => x.StandardCode = 3)
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
