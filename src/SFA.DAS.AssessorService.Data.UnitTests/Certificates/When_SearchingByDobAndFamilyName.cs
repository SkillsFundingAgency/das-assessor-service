using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using FizzWare.NBuilder;
using SFA.DAS.AssessorService.TestHelper;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class When_SearchingByDobAndFamilyName
    {
        private CertificateRepository _certificateRepository;

        [Test]
        public async Task Returns_Standard_And_Framework_Matches_And_Respects_Exclude()
        {
            // Arrange
            var dob = new DateTime(1990, 1, 1);
            var familyName = "Smith";

            var frameworkLearners = Builder<FrameworkLearner>.CreateListOfSize(2)
                .TheFirst(1)
                    .With(l => l.Id = Guid.NewGuid())
                    .With(l => l.ApprenticeULN = 1111111111)
                    .With(l => l.CertificateFamilyName = "Smith")
                    .With(l => l.ApprenticeDoB = dob)
                    .With(l => l.TrainingCode = "T100")
                    .With(l => l.FrameworkName = "Framework A")
                    .With(l => l.ApprenticeshipLevelName = "2")
                    .With(l => l.CertificationDate = new DateTime(2020,1,1))
                    .With(l => l.ProviderName = "ProvA")
                    .With(l => l.Ukprn = "12345")
                .TheNext(1)
                    .With(l => l.Id = Guid.NewGuid())
                    .With(l => l.ApprenticeULN = (long?)null)
                    .With(l => l.CertificateFamilyName = "Smith")
                    .With(l => l.ApprenticeDoB = dob)
                    .With(l => l.TrainingCode = "T200")
                    .With(l => l.FrameworkName = "Framework B")
                    .With(l => l.ApprenticeshipLevelName = "3")
                    .With(l => l.CertificationDate = new DateTime(2019,1,1))
                    .With(l => l.ProviderName = "ProvB")
                    .With(l => l.Ukprn = "54321")
                .Build().ToList();

            var standardCertificates = Builder<Certificate>.CreateListOfSize(2)
                .TheFirst(1)
                    .With(c => c.Uln = 2222222222)
                    .With(c => c.DateOfBirth = dob)
                    .WithPrivate(c => c.LatestEPAOutcome, "Pass")
                    .WithPrivate(c => c.AchievementDate, new DateTime(2021,1,1))
                    .WithPrivate(c => c.ProviderName, "ProvStd")
                    .With(c => c.ProviderUkPrn = 9999)
                    .With(c => c.Status = "Completed")
                    .With(c => c.StandardCode = 100)
                    .WithPrivate(c => c.StandardName, "Standard A")
                    .WithPrivate(c => c.StandardLevel, 3)
                    .With(c => c.Type = "Standard")
                    .WithPrivate(c => c.CertificateFamilyName, "Smith")
                .TheNext(1)
                    .With(c => c.Uln = 0)
                    .With(c => c.DateOfBirth = dob)
                    .WithPrivate(c => c.LatestEPAOutcome, "Pass")
                    .WithPrivate(c => c.AchievementDate, new DateTime(2018,1,1))
                    .WithPrivate(c => c.ProviderName, "ProvStd2")
                    .With(c => c.ProviderUkPrn = 8888)
                    .With(c => c.Status = "Completed")
                    .With(c => c.StandardCode = 200)
                    .WithPrivate(c => c.StandardName, "Standard B")
                    .WithPrivate(c => c.StandardLevel, 2)
                    .With(c => c.Type = "Standard")
                    .WithPrivate(c => c.CertificateFamilyName, "Smith")
                .Build().ToList();

            var mockDbContext = new Mock<AssessorDbContext>();
            mockDbContext.Setup(c => c.FrameworkLearners).ReturnsDbSet(frameworkLearners.AsQueryable());
            mockDbContext.Setup(c => c.StandardCertificates).ReturnsDbSet(standardCertificates.AsQueryable());

            var mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            mockAssessorUnitOfWork.SetupGet(x => x.AssessorDbContext).Returns(mockDbContext.Object);

            _certificateRepository = new CertificateRepository(mockAssessorUnitOfWork.Object);

            // Act
            var results = await _certificateRepository.SearchByDobAndFamilyName(dob, familyName, new long[] { 1111111111 });

            // Assert
            results.Should().NotBeNull();
            results.Any(r => r.Uln == 2222222222).Should().BeTrue();
            results.Any(r => r.Uln == 1111111111).Should().BeFalse();
            results.All(r => r.Uln > 0).Should().BeTrue();
        }

        [Test]
        public async Task Returns_Framework_Match_When_Not_Excluded()
        {
            // Arrange
            var dob = new DateTime(1990, 1, 1);
            var familyName = "Smith";

            var frameworkLearners = new List<FrameworkLearner>
            {
                new FrameworkLearner
                {
                    Id = Guid.NewGuid(),
                    ApprenticeULN = 3333333333,
                    CertificateFamilyName = "Smith",
                    ApprenticeDoB = dob,
                    TrainingCode = "T300",
                    FrameworkName = "Framework C",
                    ApprenticeshipLevelName = "4",
                    CertificationDate = new DateTime(2022,1,1),
                    ProviderName = "ProvC",
                    Ukprn = "77777"
                }
            };

            var mockDbContext = new Mock<AssessorDbContext>();
            mockDbContext.Setup(c => c.FrameworkLearners).ReturnsDbSet(frameworkLearners.AsQueryable());
            mockDbContext.Setup(c => c.StandardCertificates).ReturnsDbSet(new List<Certificate>().AsQueryable());

            var mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            mockAssessorUnitOfWork.SetupGet(x => x.AssessorDbContext).Returns(mockDbContext.Object);

            _certificateRepository = new CertificateRepository(mockAssessorUnitOfWork.Object);

            // Act
            var results = await _certificateRepository.SearchByDobAndFamilyName(dob, familyName, null);

            // Assert
            results.Should().ContainSingle(r => r.Uln == 3333333333);
        }

        [Test]
        public async Task Returns_Standard_Match_When_Not_Excluded()
        {
            // Arrange
            var dob = new DateTime(1990, 1, 1);
            var familyName = "Smith";

            var standardCertificates = Builder<Certificate>.CreateListOfSize(1)
                .TheFirst(1)
                    .With(c => c.Uln = 4444444444)
                    .With(c => c.DateOfBirth = dob)
                    .WithPrivate(c => c.LatestEPAOutcome, "Pass")
                    .WithPrivate(c => c.CertificateFamilyName, "Smith")
                .Build().ToList();

            var mockDbContext = new Mock<AssessorDbContext>();
            mockDbContext.Setup(c => c.FrameworkLearners).ReturnsDbSet(new List<FrameworkLearner>().AsQueryable());
            mockDbContext.Setup(c => c.StandardCertificates).ReturnsDbSet(standardCertificates.AsQueryable());

            var mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            mockAssessorUnitOfWork.SetupGet(x => x.AssessorDbContext).Returns(mockDbContext.Object);

            _certificateRepository = new CertificateRepository(mockAssessorUnitOfWork.Object);

            // Act
            var results = await _certificateRepository.SearchByDobAndFamilyName(dob, familyName, null);

            // Assert
            results.Should().ContainSingle(r => r.Uln == 4444444444);
        }
    }
}
