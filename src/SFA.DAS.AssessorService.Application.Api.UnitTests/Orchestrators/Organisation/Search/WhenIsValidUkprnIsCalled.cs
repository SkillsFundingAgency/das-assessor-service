using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Helpers;
using SFA.DAS.AssessorService.Application.Api.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using SFA.DAS.AssessorService.Application.Infrastructure;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Orchestrators.OrganisationSearch
{
    public class WhenIsValidUkprnIsCalled
    {
        private Mock<ILogger<OrganisationSearchOrchestrator>> _logger;
        private Mock<IRoatpApiClient> _roatpApiClient;
        private Mock<IReferenceDataApiClient> _referenceDataApiClient;
        private Mock<IMediator> _mediator;
        private Mock<IRegexHelper> _regexHelper;

        [TestCase("", 0, false)]
        [TestCase("notanumber", 0, false)]
        [TestCase("9999999", 9999999, false)]
        [TestCase("100000001", 100000001, false)]
        [TestCase("10000000", 10000000, true)]
        [TestCase("54321234", 54321234, true)]
        [TestCase("99999999", 99999999, true)]
        public void ThenValidityIsCheckedCorrectly(string stringToCheck, int ukprnExpected, bool isValid)
        {
            // Arrange
            _logger = new Mock<ILogger<OrganisationSearchOrchestrator>>();
            _roatpApiClient = new Mock<IRoatpApiClient>();
            _referenceDataApiClient = new Mock<IReferenceDataApiClient>();
            _mediator = new Mock<IMediator>();
            _regexHelper = new Mock<IRegexHelper>();

            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _regexHelper.Object);
            var result = sut.IsValidUkprn(stringToCheck, out int ukprnActual);

            // Assert
            result.Should().Be(isValid);
            ukprnActual.Should().Be(ukprnExpected);
        }
    }
}
