using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using SFA.DAS.AssessorService.Application.Infrastructure;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Orchestrators.OrganisationSearch
{
    public class WhenIsValidUkprnIsCalled
    {
        private Mock<ILogger<OrganisationSearchOrchestrator>> _logger;
        private Mock<IRoatpApiClient> _roatpApiClient;
        private Mock<IReferenceDataApiClient> _referenceDataApiClient;
        private Mock<IMediator> _mediator;
        private Mock<IEpaOrganisationValidator> _epaOrganisationValidator;

        [TestCase("100000001")]
        [TestCase("54321234")]
        [TestCase("99999999")]
        public void ThenValidityIsCheckedWhenNumber(string stringToCheck)
        {
            // Arrange
            _logger = new Mock<ILogger<OrganisationSearchOrchestrator>>();
            _roatpApiClient = new Mock<IRoatpApiClient>();
            _referenceDataApiClient = new Mock<IReferenceDataApiClient>();
            _mediator = new Mock<IMediator>();
            _epaOrganisationValidator = new Mock<IEpaOrganisationValidator>();

            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _epaOrganisationValidator.Object);
            var result = sut.IsValidUkprn(stringToCheck, out int ukprnActual);

            // Assert
            Assert.That(int.TryParse(stringToCheck, out int number), Is.True);
            _epaOrganisationValidator.Verify(p => p.ValidateUkprn(number), Times.Once());
        }

        [TestCase("")]
        [TestCase("notanumber")]
        public void ThenValidityIsNotCheckedWhenNotANumber(string stringToCheck)
        {
            // Arrange
            _logger = new Mock<ILogger<OrganisationSearchOrchestrator>>();
            _roatpApiClient = new Mock<IRoatpApiClient>();
            _referenceDataApiClient = new Mock<IReferenceDataApiClient>();
            _mediator = new Mock<IMediator>();
            _epaOrganisationValidator = new Mock<IEpaOrganisationValidator>();

            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _epaOrganisationValidator.Object);
            var result = sut.IsValidUkprn(stringToCheck, out int ukprnActual);

            // Assert
            Assert.That(int.TryParse(stringToCheck, out int number), Is.False);
            _epaOrganisationValidator.Verify(p => p.ValidateUkprn(It.IsAny<long?>()), Times.Never());
        }
    }
}
