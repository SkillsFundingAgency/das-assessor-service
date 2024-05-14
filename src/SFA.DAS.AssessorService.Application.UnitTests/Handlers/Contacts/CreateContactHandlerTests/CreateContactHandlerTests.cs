using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ContactHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Contacts.CreateContactHandlerTests
{

    [TestFixture]
    public class CreateContactHandlerTests
    {
        private Mock<IContactRepository> _mockContactRepository;
        private Mock<ILogger<CreateContactHandler>> _mockLogger;
        private CreateContactHandler _sut;

        [SetUp]
        public void Setup()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _mockLogger = new Mock<ILogger<CreateContactHandler>>();
            _sut = new CreateContactHandler(_mockContactRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_WhenContactDoesNotExist_ShouldCreateContact()
        {
            // Arrange
            var request = new CreateContactRequest
            {
                Email = "test@example.com",
                Username = "TestUser",
                DisplayName = "Test User",
                GovIdentifier = "12345"
            };
            
            _mockContactRepository.Setup(x => x.GetContact(request.Email)).ReturnsAsync((Contact)null);
            _mockContactRepository.Setup(x => x.CreateNewContact(It.IsAny<Contact>())).ReturnsAsync(new Contact { Id = Guid.NewGuid() });

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Result.Should().BeTrue();
            _mockContactRepository.Verify(x => x.CreateNewContact(It.IsAny<Contact>()), Times.Once);
            _mockContactRepository.Verify(x => x.UpdateGovUkIdentifier(It.IsAny<Guid>(), request.GovIdentifier), Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task Handle_WhenContactDoesNotExist_AndGovUkIdentifierIsNotSet_ShouldNotUpdateSignInId(string govIdentifier)
        {
            // Arrange
            var request = new CreateContactRequest
            {
                Email = "test@example.com",
                Username = "TestUser",
                DisplayName = "Test User",
                GovIdentifier = govIdentifier
            };

            _mockContactRepository.Setup(x => x.GetContact(request.Email)).ReturnsAsync((Contact)null);
            _mockContactRepository.Setup(x => x.CreateNewContact(It.IsAny<Contact>())).ReturnsAsync(new Contact { Id = Guid.NewGuid() });

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Result.Should().BeTrue();
            _mockContactRepository.Verify(x => x.UpdateGovUkIdentifier(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            VerifyLogger(LogLevel.Error, Times.Never);
        }

        [Test]
        public async Task Handle_WhenContactExists_ShouldUpdateSignInId()
        {
            // Arrange
            var request = new CreateContactRequest
            {
                Email = "existing@example.com",
                Username = "ExistingUser",
                DisplayName = "Existing User",
                GovIdentifier = "12345"
            };
            
            _mockContactRepository.Setup(x => x.GetContact(request.Email)).ReturnsAsync(new Contact { Id = Guid.NewGuid() });

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Result.Should().BeTrue();
            _mockContactRepository.Verify(x => x.UpdateGovUkIdentifier(It.IsAny<Guid>(), request.GovIdentifier), Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task Handle_WhenContactExists_AndGovUkIdentifierIsNotSet_ShouldNotUpdateSignInId(string govIdentifier)
        {
            // Arrange
            var request = new CreateContactRequest
            {
                Email = "existing@example.com",
                Username = "ExistingUser",
                DisplayName = "Existing User",
                GovIdentifier = govIdentifier
            };

            _mockContactRepository.Setup(x => x.GetContact(request.Email)).ReturnsAsync(new Contact { Id = Guid.NewGuid() });

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Result.Should().BeTrue();
            _mockContactRepository.Verify(x => x.UpdateGovUkIdentifier(It.IsAny<Guid>(), request.GovIdentifier), Times.Never);
            VerifyLogger(LogLevel.Error, Times.Never);
        }

        [Test]
        public void Handle_WhenExceptionIsThrown_ShouldLogErrorAndThrow()
        {
            // Arrange
            var request = new CreateContactRequest
            {
                Email = "error@example.com",
                Username = "ErrorUser"
            };

            _mockContactRepository.Setup(x => x.GetContact(request.Email))
                .Throws(new Exception("Repository exception"));

            // Act
            Func<Task> act = async () => await _sut.Handle(request, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("Repository exception");
            VerifyLogger(LogLevel.Error, Times.Once);
        }

        private void VerifyLogger(LogLevel logLevel, Func<Times> times)
        {
            _mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(p => p == logLevel),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                times);
        }
    }
}