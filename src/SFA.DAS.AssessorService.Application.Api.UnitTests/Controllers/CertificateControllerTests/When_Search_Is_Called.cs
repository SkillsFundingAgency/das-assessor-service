using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Controllers;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.CertificateControllerTests
{
    public class When_Search_Is_Called
    {
        private Mock<IMediator> _mediator;
        private CertificateController _controller;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _controller = new CertificateController(_mediator.Object);
        }

        [Test]
        public async Task Then_Returns_Ok_With_Matches_When_Request_Is_Valid()
        {
            // Arrange
            var dob = new DateTime(1990, 1, 1);
            var name = "Smith";
            var exclude = new long[] { 1111111111L };

            var repoResults = new List<SearchCertificatesResponse>
            {
                new SearchCertificatesResponse { Uln = 2222222222L, CertificateType = "Standard" }
            };

            _mediator
                .Setup(m => m.Send(It.IsAny<IRequest<List<SearchCertificatesResponse>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(repoResults);

            // Act
            var actionResult = await _controller.Search(dob, name, exclude) as OkObjectResult;

            // Assert
            Assert.IsNotNull(actionResult);
            _mediator.Verify(m => m.Send(It.Is<SearchCertificatesRequest>(r => r.DateOfBirth == dob && r.Name == name && r.Exclude == exclude), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Then_Returns_BadRequest_When_Dob_Is_Default()
        {
            // Arrange
            var dob = default(DateTime);
            var name = "Smith";

            // Act
            var actionResult = await _controller.Search(dob, name, null) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(actionResult);
            var value = actionResult.Value as IDictionary<string, string>;
            Assert.IsTrue(value.ContainsKey("DateOfBirth"));
            _mediator.Verify(m => m.Send(It.IsAny<IRequest<List<SearchCertificatesResponse>>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Then_Returns_BadRequest_When_Name_Is_Empty()
        {
            // Arrange
            var dob = new DateTime(1990, 1, 1);
            var name = "";

            // Act
            var actionResult = await _controller.Search(dob, name, null) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(actionResult);
            var value = actionResult.Value as IDictionary<string, string>;
            Assert.IsTrue(value.ContainsKey("Name"));
            _mediator.Verify(m => m.Send(It.IsAny<IRequest<List<SearchCertificatesResponse>>>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
