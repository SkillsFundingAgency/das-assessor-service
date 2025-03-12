using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs.Certificate;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQueryGetAssessmentsTests
    {
        private Mock<ICertificateRepository> _certificateRepositoryMock;
        private GetAssessmentsHandler _sut;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Setup()
        {
            _certificateRepositoryMock = new Mock<ICertificateRepository>();
            _sut = new GetAssessmentsHandler(_certificateRepositoryMock.Object);
            _cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task Handle_Should_Call_Repository_With_Correct_Parameters()
        {
            // Arrange
            var request = new GetAssessmentsRequest { Ukprn = 12345678, StandardReference = "ST0123" };
            var repositoryResult = new AssessmentsResult { EarliestAssessment = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), EndpointAssessmentCount = 5 };

            _certificateRepositoryMock
                .Setup(repo => repo.GetAssessments(request.Ukprn, request.StandardReference))
                .ReturnsAsync(repositoryResult);

            // Act
            var response = await _sut.Handle(request, _cancellationToken);

            // Assert
            response.Should().NotBeNull();
            response.EarliestAssessment.Should().Be(repositoryResult.EarliestAssessment);
            response.EndpointAssessmentCount.Should().Be(repositoryResult.EndpointAssessmentCount);

            _certificateRepositoryMock.Verify(repo =>
                repo.GetAssessments(request.Ukprn, request.StandardReference), Times.Once);
        }

        [Test]
        public async Task Handle_Should_Return_Default_Values_When_Repository_Returns_Empty_Result()
        {
            // Arrange
            var request = new GetAssessmentsRequest { Ukprn = 12345678, StandardReference = "ST0123" };
            var emptyResult = new AssessmentsResult { EarliestAssessment = null, EndpointAssessmentCount = 0 };

            _certificateRepositoryMock
                .Setup(repo => repo.GetAssessments(request.Ukprn, request.StandardReference))
                .ReturnsAsync(emptyResult);

            // Act
            var response = await _sut.Handle(request, _cancellationToken);

            // Assert
            response.Should().NotBeNull();
            response.EarliestAssessment.Should().BeNull();
            response.EndpointAssessmentCount.Should().Be(0);

            _certificateRepositoryMock.Verify(repo =>
                repo.GetAssessments(request.Ukprn, request.StandardReference), Times.Once);
        }
    }
}
