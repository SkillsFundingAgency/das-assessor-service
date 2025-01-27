using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.TestHelper;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemGetsCertificateByCertificateReferenceAndFamilyNameAndAchievementDate
    {
        private CertificateRepository _certificateRepository;
        private Mock<AssessorDbContext> _mockDbContext;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private DateTime _achievementDate;


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
            var result = await _certificateRepository.GetCertificate("0283839292", "Hawkins", _achievementDate);
            
            result.Uln.Should().Be(2222222222);
        }

        [Test]
        public async Task And_FamilyNameIsNotCorrect_Then_ReturnNull()
        {
            var result = await _certificateRepository.GetCertificate("0283839292", "Incorrect", _achievementDate);

            result.Should().BeNull();
        }

        [Test]
        public async Task And_CertificateReferenceIsIncorrect_Then_ReturnNull()
        {
            var result = await _certificateRepository.GetCertificate("09999999999", "Hawkins", _achievementDate);

            result.Should().BeNull();
        }

        [Test]
        public async Task And_AchievementDateIsIncorrect_Then_ReturnNull()
        {
            var result = await _certificateRepository.GetCertificate("0283839292", "Hawkins", DateTime.Now.AddDays(-9).Date);

            result.Should().BeNull();
        }

        private Mock<AssessorDbContext> CreateMockDbContext()
        {
            var mockDbContext = new Mock<AssessorDbContext>();

            _achievementDate = DateTime.Now.Date;

            var certificates = Builder<Certificate>.CreateListOfSize(3)
                .TheFirst(1)
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.CertificateReference = "018383838")
                .With(x => x.Uln = 1111111111)
                .WithPrivate(x => x.LearnerFamilyName, "Mirkwood")
                .WithPrivate(x => x.AchievementDate, DateTime.Now.AddDays(-1).Date)
                .TheNext(1)
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.CertificateReference = "0283839292")
                .With(x => x.Uln = 2222222222)
                .WithPrivate(x => x.LearnerFamilyName, "Hawkins")
                .WithPrivate(x => x.AchievementDate, _achievementDate.Date)
                .TheNext(1)
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.CertificateReference = "0383838272")
                .With(x => x.Uln = 3333333333)
                .WithPrivate(x => x.LearnerFamilyName, "Cornwallis")
                .WithPrivate(x => x.AchievementDate, DateTime.Now.AddDays(1).Date)
                .Build()
                .AsQueryable();

            mockDbContext.Setup(x => x.Certificates).ReturnsDbSet(certificates);

            return mockDbContext;
        }
    }
}