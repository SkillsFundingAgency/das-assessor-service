using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Certificates.Query
{
    public class WhenGetCertificatesHistory
    {
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            var mediator = new Mock<IMediator>();

            MappingBootstrapper.Initialize();
            var certificateHistoryResponses = Builder<CertificateSummaryResponse>.CreateListOfSize(10).Build().ToList();

            var certificateHistoryPaginatedList =
                new PaginatedList<CertificateSummaryResponse>(certificateHistoryResponses, 40, 1, 10);

            mediator.Setup(q => q.Send(Moq.It.IsAny<GetCertificateHistoryRequest>(), new CancellationToken()))
                .Returns(Task.FromResult((certificateHistoryPaginatedList)));

            var certificateQueryControler = new CertificateQueryController(mediator.Object);

            var statuses = new List<string> {"Submitted"};

            int page = 1;
            string userName = "testUser";
            _result = certificateQueryControler.GetCertificatesHistory(page, userName).Result;
        }

        [Test]
        public void ThenShouldCallQuery()
        {
            var result = _result as OkObjectResult;
            var paginatedList = result.Value as PaginatedList<CertificateSummaryResponse>;

            paginatedList.Items.Count.Should().Be(10);
        }
    }
}
