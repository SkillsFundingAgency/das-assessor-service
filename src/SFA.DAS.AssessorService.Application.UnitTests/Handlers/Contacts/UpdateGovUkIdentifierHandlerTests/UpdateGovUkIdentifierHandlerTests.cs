using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ContactHandlers.UpdateGovUkidentifier;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Contacts.UpdateGovUkIdentifierHandlerTests
{

    [TestFixture]
    public class UpdateGovUkIdentifierHandlerTests
    {
        private Mock<IContactRepository> _mockContactRepository;
        private Mock<IContactQueryRepository> _mockContactQueryRepository;
        private UpdateGovUkidentifierHandler _sut;

        [SetUp]
        public void Setup()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _mockContactQueryRepository = new Mock<IContactQueryRepository>();
            _sut = new UpdateGovUkidentifierHandler(_mockContactRepository.Object, _mockContactQueryRepository.Object);
        }

        [Test]
        public async Task Handle_WhenCalled()
        {
            // Arrange
            var request = new UpdateGovUkIdentifierRequest(new Guid(), "ABCDE");
            _mockContactQueryRepository.Setup(x => x.GetContactById(It.IsAny<Guid>())).ReturnsAsync(new Contact { Id = Guid.NewGuid() });

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            _mockContactQueryRepository.Verify(x => x.GetContactById(It.IsAny<Guid>()), Times.Once);
            _mockContactRepository.Verify(x => x.UpdateGovUkIdentifier(It.IsAny<Guid>(), request.GovIdentifier), Times.Once);
            _mockContactRepository.Verify(x => x.UpdateStatus(It.IsAny<Guid>(), ContactStatus.Live), Times.Once);
        }
    }
}