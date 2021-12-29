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
        private const string EpaoOrganisationId = "EPA0001";

        private Mock<IMediator> _mockMediator;

        private LearnerDetailsQueryController _sut;

        [SetUp]
        public void Arrange()
        {
            _mockMediator = new Mock<IMediator>();

            _sut = new LearnerDetailsQueryController(_mockMediator.Object);
        }

        [Test]
        public async Task And_StandardCodeIsIncluded_Then_ShouldCallQuery()
        {
            var result = await _sut.GetPipelinesCountForStandard(EpaoOrganisationId, 287) as OkObjectResult;

            _mockMediator.Verify(r => r.Send(It.Is<GetPipelinesCountRequest>(q => q.StandardCode == 287 && q.EpaoId == EpaoOrganisationId), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task And_StandardCodeIsNotIncluded_Then_ShouldCallQuery()
        {
            var result = await _sut.GetPipelinesCount(EpaoOrganisationId) as OkObjectResult;

            _mockMediator.Verify(r => r.Send(It.Is<GetPipelinesCountRequest>(q => q.StandardCode == null && q.EpaoId == EpaoOrganisationId), It.IsAny<CancellationToken>()));
        }
    }
}
