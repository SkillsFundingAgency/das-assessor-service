using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Learner;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.LearnerDetails.GetApprenticeLearnerHandlerTests
{
    [TestFixture]
    public class When_called
    {
        private GetApprenticeLearnerHandler _sut;
        private Mock<ILearnerRepository> _mockLearnerRepository;
        private Mock<ILogger<GetApprenticeLearnerHandler>> _mockLogger;

        [SetUp]
        public void Arrange()
        {
            _mockLearnerRepository = new Mock<ILearnerRepository>();
            _mockLogger = new Mock<ILogger<GetApprenticeLearnerHandler>>();
            _sut = new GetApprenticeLearnerHandler(_mockLearnerRepository.Object, _mockLogger.Object); 
        }

        [Test, MoqAutoData]
        public async Task ThenLearnerIsRetrievedFromDatabase(GetApprenticeLearnerRequest request, ApprenticeLearner learner)
        {
            // Arrange
            learner.OverallGrade = "Merit";
            _mockLearnerRepository
                .Setup(r => r.Get(request.ApprenticeshipId))
                .ReturnsAsync(learner);

            // Act
            var result = await _sut.Handle(request, new CancellationToken());

            // Assert
            _mockLearnerRepository
                .Verify(r => r.Get(request.ApprenticeshipId), Times.Once);

            result.Should().BeEquivalentTo(new
            {
                ApprenticeshipId = learner.ApprenticeshipId.Value,
                Ukprn = learner.UkPrn,
                learner.LearnStartDate,
                learner.PlannedEndDate,
                StandardCode = learner.StdCode,
                learner.StandardUId,
                learner.StandardReference,
                learner.StandardName,
                learner.CompletionStatus,
                Outcome = "Pass",
                learner.ApprovalsStopDate,
                learner.ApprovalsPauseDate,
                learner.EstimatedEndDate,
                learner.Uln,
                learner.GivenNames,
                learner.FamilyName,
                learner.AchievementDate,
                learner.ProviderName
            });
        }

        [Test]
        [MoqInlineAutoData(null, null)]
        [MoqInlineAutoData("Pass","Pass")]
        [MoqInlineAutoData("Credit", "Pass")]
        [MoqInlineAutoData("Merit", "Pass")]
        [MoqInlineAutoData("Distinction", "Pass")]
        [MoqInlineAutoData("PassWithExcellence", "Pass")]
        [MoqInlineAutoData("Outstanding", "Pass")]
        [MoqInlineAutoData("NoGradeAwarded", "Pass")]
        [MoqInlineAutoData("Fail","Fail")]
        public async Task ThenLearnerGradeIsMappedSuccessfully(string grade, string outcome, GetApprenticeLearnerRequest request, ApprenticeLearner learner)
        {
            // Arrange
            learner.OverallGrade = grade;
            _mockLearnerRepository
                .Setup(r => r.Get(request.ApprenticeshipId))
                .ReturnsAsync(learner);

            // Act
            var result = await _sut.Handle(request, new CancellationToken());

            // Assert
            _mockLearnerRepository
                .Verify(r => r.Get(request.ApprenticeshipId), Times.Once);

            result.Should().BeEquivalentTo(new
            {
                Outcome = outcome
            });
        }

    }
}