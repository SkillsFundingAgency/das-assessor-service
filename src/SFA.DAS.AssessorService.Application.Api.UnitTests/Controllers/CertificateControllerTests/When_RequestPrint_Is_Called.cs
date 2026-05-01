using System;
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
    public class When_RequestPrint_Is_Called
    {
        private Mock<IMediator> _mediator;
        private CertificateController _controller;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator
                .Setup(m => m.Send(It.IsAny<IRequest<Unit>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            _controller = new CertificateController(_mediator.Object);
        }

        [Test]
        public async Task Then_Mediator_Send_Is_Called_And_NoContent_Returned()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cmd = new UpdateCertificatePrintRequestCommand
            {
                Address = new CertificatePrintAddress { ContactName = "ABC" },
                PrintRequestedAt = DateTime.UtcNow,
                PrintRequestedBy = "User"
            };

            // Act
            var result = await _controller.RequestPrint(id, cmd) as NoContentResult;

            // Assert
            Assert.IsNotNull(result, "Expected NoContentResult when print request is successful");
            _mediator.Verify(m => m.Send(It.Is<UpdateCertificatePrintRequestCommand>(c => c.CertificateId == id), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
