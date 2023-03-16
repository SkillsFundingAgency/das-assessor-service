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
    public class WhenIsValidEpaOrganisationIdIsCalled
    {
        private Mock<ILogger<OrganisationSearchOrchestrator>> _logger;
        private Mock<IRoatpApiClient> _roatpApiClient;
        private Mock<IReferenceDataApiClient> _referenceDataApiClient;
        private Mock<IMediator> _mediator;
        private Mock<IRegexHelper> _regexHelper;

        [TestCase("EPA0001")]
        [TestCase("EPA0010")]
        [TestCase("EPA0020")]
        public void ThenValidityIsCheckedCorrectly(string organisationIdToCheck)
        {
            // Arrange
            _logger = new Mock<ILogger<OrganisationSearchOrchestrator>>();
            _roatpApiClient = new Mock<IRoatpApiClient>();
            _referenceDataApiClient = new Mock<IReferenceDataApiClient>();
            _mediator = new Mock<IMediator>();
            _regexHelper = new Mock<IRegexHelper>();

            // Act
            var sut = new OrganisationSearchOrchestrator(_logger.Object, _roatpApiClient.Object, _referenceDataApiClient.Object, _mediator.Object, _regexHelper.Object);
            sut.IsValidEpaOrganisationId(organisationIdToCheck);

            // Assert
            _regexHelper.Verify(p => p.RegexMatchSuccess(organisationIdToCheck, "[eE][pP][aA][0-9]{4,9}$"));
        }
    }
}
