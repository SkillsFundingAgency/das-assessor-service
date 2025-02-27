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
    public class WhenSystemsGetsCertificateByUlnAndFamilyName
    {
        private CertificateRepository _certificateRepository;
        private Certificate _result;

        [SetUp]
        public void Arrange()
        {
            var mockDbContext = CreateMockDbContext();
            var unitOfWork = new Mock<IAssessorUnitOfWork>();
            unitOfWork
                .SetupGet(x => x.AssessorDbContext)
                .Returns(mockDbContext.Object);

            _certificateRepository = new CertificateRepository(unitOfWork.Object);

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
                .With(x => x.CertificateData = GetCertificateData("Mirkwood"))
                .WithPrivate(x => x.LearnerFamilyName, "Mirkwood")
                .TheNext(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 22222222222)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0002")
                .With(x => x.CertificateData = GetCertificateData("Hawkins"))
                .WithPrivate(x => x.LearnerFamilyName, "Hawkins")
                .TheNext(8)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 33333333333)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0003")
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
