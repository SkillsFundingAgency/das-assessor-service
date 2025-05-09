﻿using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemGetsCertificateByUlnAndStandardCode
    {
        private CertificateRepository _certificateRepository;
        private Certificate _result;

        [SetUp]
        public void Arrange()
        {
            var mockDbContext = CreateMockDbContext();
            var mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            mockAssessorUnitOfWork
                .SetupGet(x => x.AssessorDbContext)
                .Returns(mockDbContext.Object);

            _certificateRepository = new CertificateRepository(mockAssessorUnitOfWork.Object);
        
            _result = _certificateRepository.GetCertificate(22222222222, 93).Result;
        }

        [Test]
        public void ItShouldReturnResult()
        {
            _result.Uln.Should().Be(22222222222);
            _result.StandardCode.Should().Be(93);
        }

        private static Mock<IAssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<IAssessorDbContext>();

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

            mockDbContext.Setup(c => c.StandardCertificates).ReturnsDbSet(certificates);
            return mockDbContext;
        }
    }
}

