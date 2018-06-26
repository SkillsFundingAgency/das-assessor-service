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
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Certificates.Query
{
    public class WhenGetCertificateAddresses
    {
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            var mediator = new Mock<IMediator>();

            MappingBootstrapper.Initialize();
            var certificateResponses = Builder<CertificateAddress>.CreateListOfSize(10).Build().ToList();

            mediator.Setup(q => q.Send(Moq.It.IsAny<GetPreviousAddressesRequest>(), new CancellationToken()))
                .Returns(Task.FromResult((certificateResponses)));

            var certificateQueryControler = new CertificateQueryController(mediator.Object);
            
            _result = certificateQueryControler.GetPreviousAddresses("TestUser").Result;
        }

        [Test]
        public void ThenShouldCallQuery()
        {
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }
    }
}
