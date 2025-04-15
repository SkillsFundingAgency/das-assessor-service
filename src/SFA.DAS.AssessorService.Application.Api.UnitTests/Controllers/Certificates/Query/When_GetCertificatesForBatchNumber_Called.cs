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
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Certificates.Query
{
    public class When_GetCertificatesForBatchNumber_Called
    {
        private IActionResult _result;
        private int _batchNumber = 222;

        [SetUp]
        public void Arrange()
        {
            var mediator = new Mock<IMediator>();

            var certificateResponses = new CertificatesForBatchNumberResponse()
            {
                Certificates = Builder<CertificatePrintSummaryBase>.CreateListOfSize(10).Build().ToList()
            };

            mediator.Setup(q => q.Send(Moq.It.IsAny<GetCertificatesForBatchNumberRequest>(), new CancellationToken()))
                .Returns(Task.FromResult((certificateResponses)));

            var certificateQueryControler = new CertificateQueryController(mediator.Object);

            _result = certificateQueryControler.GetCertificatesForBatchNumber(_batchNumber).Result;
        }

        [Test]
        public void ThenShouldCallQuery()
        {
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }
    }
}
