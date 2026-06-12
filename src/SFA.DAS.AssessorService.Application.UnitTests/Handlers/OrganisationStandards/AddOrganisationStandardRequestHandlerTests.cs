using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.OrganisationStandards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Handlers.OrganisationStandards;
using System;
using System.Collections.Generic;
using System.Threading;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;
using DeliveryArea = SFA.DAS.AssessorService.Api.Types.Models.AO.DeliveryArea;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.OrganisationStandards
{
    [TestFixture]
    public class AddOrganisationStandardRequestHandlerTests
    {
        private Mock<IStandardService> _mockStandardService;
        private Mock<IMediator> _mockMediator;
        private AddOrganisationStandardRequestHandler _handler;

        private OrganisationStandardAddRequest _request;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _request = new OrganisationStandardAddRequest
            {
                OrganisationId = Guid.NewGuid().ToString(),
                StandardReference = "ST0001",
                StandardVersions = new List<string> { "1.0", "1.1", "1.2" },
                ContactId = Guid.NewGuid()
            };

            _mockStandardService = new Mock<IStandardService>();
            _mockMediator = new Mock<IMediator>();

            _handler = new AddOrganisationStandardRequestHandler(_mockMediator.Object, _mockStandardService.Object);
        }

        [Test]
        public async Task Handle_ShouldAddOrganisationStandard_WhenStandardExists()
        {
            // Arrange
            var standard = new Standard { LarsCode = 12345 };

            var deliveryAreas = new List<DeliveryArea> { new DeliveryArea { Id = 1 }, new DeliveryArea { Id = 2 } };

            _mockStandardService.Setup(repo => repo.GetStandardVersionsByIFateReferenceNumber(It.IsAny<string>())).ReturnsAsync(new List<Standard> { standard });
            _mockMediator.Setup(med => med.Send(It.IsAny<GetDeliveryAreasRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(deliveryAreas);

            // Act
            var result = await _handler.Handle(_request, new CancellationToken());

            // Assert
            _mockMediator.Verify(med => med.Send(It.IsAny<CreateEpaOrganisationStandardRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockMediator.Verify(med => med.Send(It.IsAny<SendAddStandardEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_ShouldThrowException_WhenStandardDoesNotExist()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(_request, new CancellationToken()));
        }
    }
}
