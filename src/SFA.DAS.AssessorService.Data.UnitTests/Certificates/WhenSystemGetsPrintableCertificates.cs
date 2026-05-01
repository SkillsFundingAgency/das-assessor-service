using System;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemGetsPrintableCertificates
    {
        private CertificateRepository _certificateRepository;
        private Mock<IAssessorDbContext> _mockDbContext;
        private Mock<IAssessorUnitOfWork> _mockUnitOfWork;
        private long _uln;

        [SetUp]
        public void Arrange()
        {
            _uln = 1234567890L;

            _mockDbContext = new Mock<IAssessorDbContext>();

            var standardCertificates = Builder<Certificate>.CreateListOfSize(6)
                .TheFirst(1)
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.Uln = _uln)
                .With(x => x.StandardCode = 1)
                .With(x => x.CertificateData = new CertificateData
                {
                    StandardName = "Plumbing",
                    StandardLevel = 2,
                    AchievementDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Unspecified),
                    EpaDetails = new EpaDetails { LatestEpaOutcome = EpaOutcome.Pass }
                })
                .With(x => x.Status = CertificateStatus.Submitted)
                .TheNext(1)
                .With(x => x.Uln = _uln)
                .With(x => x.StandardCode = 2)
                .With(x => x.CertificateData = new CertificateData
                {
                    StandardName = "Electrical",
                    StandardLevel = 3,
                    AchievementDate = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Unspecified)
                })
                .With(x => x.Status = CertificateStatus.Deleted) // excluded
                .TheNext(1)
                .With(x => x.Uln = _uln)
                .With(x => x.StandardCode = 3)
                .With(x => x.CertificateData = new CertificateData
                {
                    StandardName = "Carpentry",
                    StandardLevel = 3,
                    AchievementDate = null
                })
                .With(x => x.Status = CertificateStatus.Submitted)
                .TheNext(1)
                .With(x => x.Uln = _uln)
                .With(x => x.StandardCode = 4)
                .With(x => x.CertificateData = new CertificateData
                {
                    StandardName = "NullOutcomeStandard",
                    StandardLevel = 2,
                    AchievementDate = new DateTime(2024, 5, 1, 0, 0, 0, DateTimeKind.Unspecified),
                    EpaDetails = null
                })
                .With(x => x.Status = CertificateStatus.Submitted)
                .TheNext(1)
                .With(x => x.Uln = _uln)
                .With(x => x.StandardCode = 5)
                .With(x => x.CertificateData = new CertificateData
                {
                    StandardName = "FailOutcomeStandard",
                    StandardLevel = 2,
                    AchievementDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Unspecified),
                    EpaDetails = new EpaDetails { LatestEpaOutcome = EpaOutcome.Fail }
                })
                .With(x => x.Status = CertificateStatus.Submitted)
                .TheNext(1)
                .With(x => x.Uln = 7777777777) // different ULN
                .With(x => x.CertificateData = new CertificateData
                {
                    StandardName = "Metalwork",
                    StandardLevel = 2,
                    AchievementDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
                })
                .With(x => x.Status = CertificateStatus.Submitted)
                .Build()
                .Select(c => c.WithComputedFieldsFromData())
                .AsQueryable();

            var frameworkLearners = Builder<FrameworkLearner>.CreateListOfSize(2)
                .TheFirst(1)
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.ApprenticeULN = _uln)
                .With(x => x.FrameworkName = "Advanced Framework")
                .With(x => x.TrainingCode = "FW123")
                .With(x => x.ApprenticeshipLevelName = "Level 4")
                .With(x => x.CertificationDate = new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Unspecified))
                .TheNext(1)
                .With(x => x.ApprenticeULN = 8888888888)
                .With(x => x.FrameworkName = "Different ULN")
                .With(x => x.CertificationDate = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Unspecified))
                .Build()
                .AsQueryable();

            _mockDbContext.Setup(x => x.StandardCertificates).ReturnsDbSet(standardCertificates);
            _mockDbContext.Setup(x => x.FrameworkLearners).ReturnsDbSet(frameworkLearners);

            _mockUnitOfWork = new Mock<IAssessorUnitOfWork>();
            _mockUnitOfWork.SetupGet(x => x.AssessorDbContext).Returns(_mockDbContext.Object);

            _certificateRepository = new CertificateRepository(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task Then_Returns_Expected_Printable_Certificates_Ordered_By_DateAwarded()
        {
            // Act
            var result = await _certificateRepository.GetPrintableCertificates(_uln);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            var framework = result.Single(x => x.CertificateType == "Framework");
            framework.CourseName.Should().Be("Advanced Framework");
            framework.DateAwarded.Should().Be(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Unspecified));

            var standard = result.Single(x => x.CertificateType == "Standard");
            standard.CourseName.Should().Be("Plumbing");
            standard.CourseLevel.Should().Be("2");
            standard.DateAwarded.Should().Be(new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Unspecified));

            result.Should().NotContain(x => x.CertificateType == "Standard" && x.CourseName == "NullOutcomeStandard");
            result.Should().NotContain(x => x.CertificateType == "Standard" && x.CourseName == "FailOutcomeStandard");

            result.Should().BeInDescendingOrder(x => x.DateAwarded);
        }

        [Test]
        public async Task Then_Returns_Empty_List_When_No_Matching_ULN()
        {
            // Act
            var result = await _certificateRepository.GetPrintableCertificates(9999999999);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
