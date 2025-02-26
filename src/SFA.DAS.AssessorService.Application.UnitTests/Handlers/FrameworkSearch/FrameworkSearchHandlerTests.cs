using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.FrameworkSearch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using FluentAssertions;
using System.Linq;

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

            _frameworkSearchHandler = new FrameworkSearchHandler(_frameworkLearnerRepository.Object, new Mock<ILogger<FrameworkSearchHandler>>().Object, _mapperMock.Object);
        }

        [Test, MoqAutoData]
        public async Task ThenRequestSentToFrameworkLearnerRepository(FrameworkSearchQuery query)
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
        public async Task ThenMapperIsCalled(FrameworkSearchQuery query, List<FrameworkLearner> learners)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.Search(query.FirstName, query.LastName, query.DateOfBirth))
                .ReturnsAsync(learners);

            // Act
            var result = await _frameworkSearchHandler.Handle(query, new CancellationToken());

            // Assert
            _mapperMock.Verify(m => m.Map<List<FrameworkSearchResult>>(learners), Times.Once());
        }

        [Test, MoqAutoData]
        public async Task AndNoFrameworkLearnersFoundThenEmptyListIsReturned(FrameworkSearchQuery query)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.Search(query.FirstName, query.LastName, query.DateOfBirth))
                .ReturnsAsync(new List<FrameworkLearner>());

            _mapperMock
                .Setup(m => m.Map<List<FrameworkSearchResult>>(It.IsAny<List<FrameworkLearner>>()))
                .Returns(new List<FrameworkSearchResult>());

            // Act
            var result = await _frameworkSearchHandler.Handle(query, new CancellationToken());

            // Assert
            result.Should().BeEmpty();
        }

        [Test, MoqAutoData]
        public async Task ThenLearnersAreReturnedAsTheCorrectType(FrameworkSearchQuery query, List<FrameworkLearner> learners)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.Search(query.FirstName, query.LastName, query.DateOfBirth))
                .ReturnsAsync(learners);

            var mappedResults = learners.Select(l =>
                new FrameworkSearchResult
                { 
                    ApprenticeshipLevelName = l.ApprenticeshipLevelName,
                    CertificationYear = l.CertificationYear,
                    FrameworkName = l.FrameworkName,
                    Id = l.Id
                }).ToList();

            _mapperMock
                .Setup(m => m.Map<List<FrameworkSearchResult>>(learners))
                .Returns(mappedResults);

            // Act
            var result = await _frameworkSearchHandler.Handle(query, new CancellationToken());

            // Assert
            result.Should().BeOfType<List<FrameworkSearchResult>>();
            result.Should().HaveCount(learners.Count);
        }
    }
}