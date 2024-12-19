using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemGetsCertificateByCertificateReference
    {
        private CertificateRepository _certificateRepository;
        private Mock<AssessorDbContext> _mockDbContext;
        private Mock<IUnitOfWork> _mockUnitOfWork;


        [SetUp]
        public void Arrange()
        {
            
            _mockDbContext = CreateMockDbContext();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _certificateRepository = new CertificateRepository(_mockUnitOfWork.Object, _mockDbContext.Object);
        }

        [Test]
        public async Task ItShouldReturnResult()
        {
            var result = await _certificateRepository.GetCertificate("0283839292");
            
            result.Uln.Should().Be(2222222222);
        }

        private Mock<AssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<AssessorDbContext>();

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

            mockDbContext.Setup(x => x.Certificates).ReturnsDbSet(certificates);

            return mockDbContext;
        }
    }
}