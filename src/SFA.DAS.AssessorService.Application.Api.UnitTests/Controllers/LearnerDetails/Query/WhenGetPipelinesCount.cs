using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Certificates.Query
{
    public class GetPipelinesCount
    {
        private LearnerDetailsQueryController _sut;

        [SetUp]
        public void Arrange()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(q => q.Send(It.IsAny<GetPipelinesCountRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(10));

            _sut = new LearnerDetailsQueryController(mediator.Object);
        }

        [Test]
        public async Task ThenShouldCallQuery()
        {
            // Act
            var result = await _sut.GetPipelinesCount("EPA0001", 287) as OkObjectResult;
            
            // Assert
            result.Value.Should().Be(10);
        }
    }
}
