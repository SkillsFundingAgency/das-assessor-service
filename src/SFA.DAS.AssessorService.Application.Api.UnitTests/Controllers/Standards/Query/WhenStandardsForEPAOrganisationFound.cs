using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Standards.Query
{
    public class WhenStandardsForEpaOrganisationFound
    {
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            var mediator = new Mock<IMediator>();
            var listOfEpaoRegisteredStandards =
                Builder<GetEpaoRegisteredStandardsResponse>.CreateListOfSize(10).Build().ToList();

            var registeredStandardsPaginatedList =
                new PaginatedList<GetEpaoRegisteredStandardsResponse>(listOfEpaoRegisteredStandards, 1, 1, 10);

            mediator.Setup(q => q.Send(Moq.It.IsAny<GetEpaoRegisteredStandardsRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(registeredStandardsPaginatedList));
            var epoRegisteredStandardsResult = Builder<EpoRegisteredStandardsResult>.CreateNew().Build();
            var standardRepositoryMock = new Mock<IStandardRepository>();
            standardRepositoryMock.Setup(x =>
                    x.GetEpaoRegisteredStandards(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>()))
                .Returns(Task.FromResult(epoRegisteredStandardsResult));

            var controllerLoggerMock = new Mock<ILogger<StandardQueryController>>();
            var standardQueryController =
                new StandardQueryController(mediator.Object, controllerLoggerMock.Object);

            _result = standardQueryController.GetEpaoRegisteredStandards("EPA0008", 1).Result;
        }


        public void ThenTheResultReturnsOkStatus()
        {
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }

        [Test]
        public void ThenTheQueryReturnsTheCorrectResult()
        {
            if (!(_result is OkObjectResult result))
                ((OkObjectResult)null).Should().NotBeNull();
            else
                result.Value.Should().BeOfType<PaginatedList<GetEpaoRegisteredStandardsResponse>>();

        }
    }
}
