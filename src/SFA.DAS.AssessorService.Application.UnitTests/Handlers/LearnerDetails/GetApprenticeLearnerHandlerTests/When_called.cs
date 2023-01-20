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
            _mockLearnerRepository
                .Setup(r => r.Get(request.ApprenticeshipId))
                .ReturnsAsync(learner);

            var expected = SetupResponse(learner);

            // Act
            var result = await _sut.Handle(request, new CancellationToken());

            // Assert
            _mockLearnerRepository
                .Verify(r => r.Get(request.ApprenticeshipId), Times.Once);

            result.Should().BeEquivalentTo(expected);
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