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
        [Test, MoqAutoData]
        public async Task ThenLearnerIsRetrievedFromTheDatabase(GetApprenticeLearnerRequest request, ApprenticeLearner learner)
        {
            // Arrange
            var _mockLearnerRepository = SetupMockRepository(request, learner);

            var _sut = SetupSut(_mockLearnerRepository.Object);

            // Act
            var result = await _sut.Handle(request, new CancellationToken());

            // Assert
            _mockLearnerRepository
                .Verify(r => r.Get(request.ApprenticeshipId), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task ThenLearnerIsReturnedAsTheCorrectType(GetApprenticeLearnerRequest request, ApprenticeLearner learner)
        {
            // Arrange
            var _mockLearnerRepository = SetupMockRepository(request, learner);

            var _sut = SetupSut(_mockLearnerRepository.Object);

            var expected = SetupResponse(learner);

            // Act
            var result = await _sut.Handle(request, new CancellationToken());

            // Assert
            result.Should().BeOfType<GetApprenticeLearnerResponse>();
            result.Should().BeEquivalentTo(expected);
        }

        private Mock<ILearnerRepository> SetupMockRepository(GetApprenticeLearnerRequest request, ApprenticeLearner learner)
        {
            var mockLearnerRepository = new Mock<ILearnerRepository>();

            mockLearnerRepository
                .Setup(r => r.Get(request.ApprenticeshipId))
                .ReturnsAsync(learner);

            return mockLearnerRepository;
        }

        private GetApprenticeLearnerHandler SetupSut(ILearnerRepository repository)
        {
            var mockLogger = new Mock<ILogger<GetApprenticeLearnerHandler>>();

            return new GetApprenticeLearnerHandler(repository, mockLogger.Object);
        }

        private GetApprenticeLearnerResponse SetupResponse(ApprenticeLearner learner)
        {
            return new GetApprenticeLearnerResponse
            {
                ApprenticeshipId = learner.ApprenticeshipId.Value,
                Ukprn = learner.UkPrn,
                LearnStartDate = learner.LearnStartDate,
                PlannedEndDate = learner.PlannedEndDate,
                StandardCode = learner.StdCode,
                StandardUId = learner.StandardUId,
                StandardReference = learner.StandardReference,
                StandardName = learner.StandardName,
                CompletionStatus = learner.CompletionStatus,
                ApprovalsStopDate = learner.ApprovalsStopDate,
                ApprovalsPauseDate = learner.ApprovalsPauseDate,
                EstimatedEndDate = learner.EstimatedEndDate,
                Uln = learner.Uln,
                GivenNames = learner.GivenNames,
                FamilyName = learner.FamilyName,
                LearnActEndDate = learner.LearnActEndDate,
                IsTransfer = learner.IsTransfer,
                DateTransferIdentified = learner.DateTransferIdentified,
                ProviderName = learner.ProviderName
            };
        }
    }
}