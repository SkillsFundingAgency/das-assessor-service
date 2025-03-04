using System;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemGetsCertificateByCertificateReference
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
        public async Task ItShouldReturnResult()
        {
            var result = await _certificateRepository.GetCertificate("0283839292") as Certificate;
            
            result.Uln.Should().Be(2222222222);
        }

        private static Mock<IAssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<IAssessorDbContext>();

            var certificates = Builder<Certificate>.CreateListOfSize(3)
                .TheFirst(1)
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.CertificateReference = "018383838")
                .With(x => x.Uln = 1111111111)
                .TheNext(1)
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.CertificateReference = "0283839292")
                .With(x => x.Uln = 2222222222)
                .TheNext(1)
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.CertificateReference = "0383838272")
                .With(x => x.Uln = 3333333333)
                .Build()
                .AsQueryable();

            mockDbContext.Setup(x => x.StandardCertificates).ReturnsDbSet(certificates);

            return mockDbContext;
        }
    }
}