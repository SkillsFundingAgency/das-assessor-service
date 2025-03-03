using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
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
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.FrameworkSearch
{
    [TestFixture]
    public class GetFrameworkLearnerHandlerTests : MapperBase
    {
        private GetFrameworkLearnerHandler _handler;
        private Mock<IFrameworkLearnerRepository> _frameworkLearnerRepository;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _frameworkLearnerRepository = new Mock<IFrameworkLearnerRepository>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetFrameworkLearnerHandler(_mapperMock.Object, _frameworkLearnerRepository.Object);
        }

        [Test, MoqAutoData]
        public async Task ThenRequestSentToFrameworkLearnerRepository(GetFrameworkCertificateQuery query, FrameworkLearner learner)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.GetById(query.Id))
                .ReturnsAsync(learner);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            _frameworkLearnerRepository.Verify(r => r.GetById(query.Id), Times.Once());
        }

        [Test, MoqAutoData]
        public async Task ThenMapperIsCalled(GetFrameworkCertificateQuery query, FrameworkLearner learner)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.GetById(query.Id))
                .ReturnsAsync(learner);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            _mapperMock.Verify(m => m.Map<GetFrameworkCertificateResult>(learner), Times.Once());
        }

        [Test, MoqAutoData]
        public async Task AndTheFrameworkLearnerIsNullThenDefaultIsReturned(GetFrameworkCertificateQuery query)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.GetById(query.Id))
                .ReturnsAsync((FrameworkLearner)null);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            result.Should().Be(default);
        }

        [Test, MoqAutoData]
        public async Task ThenFrameworkLearnerIsReturnedAsTheCorrectType(
            GetFrameworkCertificateQuery query, 
            FrameworkLearner learner,
            GetFrameworkCertificateResult frameworkResult)
        {
            // Arrange
            _frameworkLearnerRepository
                .Setup(r => r.GetById(query.Id))
                .ReturnsAsync(learner);

            _mapperMock
                .Setup(m => m.Map<GetFrameworkCertificateResult>(learner))
                .Returns(frameworkResult);

            // Act
            var result = await _handler.Handle(query, new CancellationToken());

            // Assert
            result.Should().BeOfType<GetFrameworkCertificateResult>();
            result.Should().BeEquivalentTo(frameworkResult);
        }
    }
}