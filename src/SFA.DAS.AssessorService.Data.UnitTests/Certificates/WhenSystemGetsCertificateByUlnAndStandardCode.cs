using System.Collections.Generic;
using System.Data;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemGetsCertificateByUlnAndStandardCode
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
        
          _result = _certificateRepository.GetCertificate(22222222222, 93).Result;
        }

        [Test]
        public void ItShouldReturnResult()
        {
            _result.Uln.Should().Be(22222222222);
            _result.StandardCode.Should().Be(93);
        }

        private Mock<AssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<AssessorDbContext>();

            var certificates = Builder<Certificate>.CreateListOfSize(10)
                .TheFirst(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 1111111111)
                .With(x => x.StandardCode = 81)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0001")
                .With(x => x.IsPrivatelyFunded = true)
                .TheNext(1)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 22222222222)
                .With(x => x.StandardCode = 93)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0002")
                .With(x => x.IsPrivatelyFunded = true)
                .TheNext(8)
                .With(x => x.Organisation = Builder<Organisation>.CreateNew().Build())
                .With(x => x.Uln = 333333333333)
                .With(x => x.Organisation.EndPointAssessorOrganisationId = "EPA0003")
                .With(x => x.StandardCode = 100)
                .With(x => x.IsPrivatelyFunded = true)
                .Build()
                .AsQueryable();

            mockDbContext.Setup(c => c.Certificates).ReturnsDbSet(certificates);
            return mockDbContext;
        }
    }
}

