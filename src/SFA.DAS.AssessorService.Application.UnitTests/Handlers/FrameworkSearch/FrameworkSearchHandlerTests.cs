using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.FrameworkSearch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using FluentAssertions;
using System.Linq;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.FrameworkSearch
{
    [TestFixture]
    public class FrameworkSearchHandlerTests : MapperBase
    {
        private FrameworkSearchHandler _frameworkSearchHandler;
        private Mock<IFrameworkLearnerRepository> _frameworkLearnerRepository;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _frameworkLearnerRepository = new Mock<IFrameworkLearnerRepository>();
            _mapperMock = new Mock<IMapper>();

            _frameworkSearchHandler = new FrameworkSearchHandler(_frameworkLearnerRepository.Object, _mapperMock.Object);
        }

        [Test, MoqAutoData]
        public async Task ThenRequestSentToFrameworkLearnerRepository(FrameworkLearnerSearchRequest query)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.Search(query.FirstName, query.LastName, query.DateOfBirth))
                .ReturnsAsync(new List<FrameworkLearner>());

            // Act
            var result = await _frameworkSearchHandler.Handle(query, new CancellationToken());

            // Assert
            _frameworkLearnerRepository.Verify(r => r.Search(query.FirstName, query.LastName, query.DateOfBirth), Times.Once());
        }

        [Test, MoqAutoData]
        public async Task ThenMapperIsCalled(FrameworkLearnerSearchRequest query, List<FrameworkLearner> learners)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.Search(query.FirstName, query.LastName, query.DateOfBirth))
                .ReturnsAsync(learners);

            // Act
            var result = await _frameworkSearchHandler.Handle(query, new CancellationToken());

            // Assert
            _mapperMock.Verify(m => m.Map<List<FrameworkLearnerSearchResponse>>(learners), Times.Once());
        }

        [Test, MoqAutoData]
        public async Task AndNoFrameworkLearnersFoundThenEmptyListIsReturned(FrameworkLearnerSearchRequest query)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.Search(query.FirstName, query.LastName, query.DateOfBirth))
                .ReturnsAsync(new List<FrameworkLearner>());

            _mapperMock
                .Setup(m => m.Map<List<FrameworkLearnerSearchResponse>>(It.IsAny<List<FrameworkLearner>>()))
                .Returns(new List<FrameworkLearnerSearchResponse>());

            // Act
            var result = await _frameworkSearchHandler.Handle(query, new CancellationToken());

            // Assert
            result.Should().BeEmpty();
        }

        [Test, MoqAutoData]
        public async Task ThenLearnersAreReturnedAsTheCorrectType(FrameworkLearnerSearchRequest query, List<FrameworkLearner> learners)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.Search(query.FirstName, query.LastName, query.DateOfBirth))
                .ReturnsAsync(learners);

            var mappedResults = learners.Select(l =>
                new FrameworkLearnerSearchResponse
                { 
                    ApprenticeshipLevelName = l.ApprenticeshipLevelName,
                    CertificationYear = l.CertificationYear,
                    FrameworkName = l.FrameworkName,
                    Id = l.Id
                }).ToList();

            _mapperMock
                .Setup(m => m.Map<List<FrameworkLearnerSearchResponse>>(learners))
                .Returns(mappedResults);

            // Act
            var result = await _frameworkSearchHandler.Handle(query, new CancellationToken());

            // Assert
            result.Should().BeOfType<List<FrameworkLearnerSearchResponse>>();
            result.Should().HaveCount(learners.Count);
        }
    }
}