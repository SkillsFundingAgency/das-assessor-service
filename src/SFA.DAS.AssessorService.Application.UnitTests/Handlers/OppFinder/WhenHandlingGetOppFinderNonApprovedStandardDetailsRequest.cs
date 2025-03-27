using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Standards;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.OppFinder
{
    public class WhenHandlingGetOppFinderNonApprovedStandardDetailsRequest
    {
        private GetOppFinderNonApprovedStandardDetailsRequest _request;
        private OppFinderNonApprovedStandardDetailsResult _nonApprovedStandardDetails;

        private Mock<IOppFinderRepository> _mockOppFinderRepository;

        private GetOppFinderNonApprovedStandardDetailsHandler _getNonApprovedStandardDetails;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _request = fixture.Create<GetOppFinderNonApprovedStandardDetailsRequest>();

            _nonApprovedStandardDetails = fixture.Build<OppFinderNonApprovedStandardDetailsResult>()
                .With(x => x.Level, fixture.Create<int>().ToString())
                .Create();

            _mockOppFinderRepository = new Mock<IOppFinderRepository>();

            _mockOppFinderRepository.Setup(r => r.GetOppFinderNonApprovedStandardDetails(_request.StandardReference))
                .ReturnsAsync(_nonApprovedStandardDetails);

            _getNonApprovedStandardDetails = new GetOppFinderNonApprovedStandardDetailsHandler(Mock.Of<ILogger<GetOppFinderNonApprovedStandardDetailsHandler>>(), _mockOppFinderRepository.Object);
        }

        [Test]
        public async Task And_RepositoryReturnsNull_Then_ReturnNull()
        {
            _mockOppFinderRepository.Setup(r => r.GetOppFinderNonApprovedStandardDetails(It.IsAny<string>()))
                .ReturnsAsync((OppFinderNonApprovedStandardDetailsResult)null);

            var result = await _getNonApprovedStandardDetails.Handle(_request, CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task Then_ReturnStandardDetailsResponse()
        {
            var result = await _getNonApprovedStandardDetails.Handle(_request, CancellationToken.None);

            result.Title.Should().Be(_nonApprovedStandardDetails.Title);
            result.StandardReference.Should().Be(_nonApprovedStandardDetails.IFateReferenceNumber);
            result.Sector.Should().Be(_nonApprovedStandardDetails.Route);
            result.TypicalDuration.Should().Be(_nonApprovedStandardDetails.TypicalDuration);
            result.Trailblazer.Should().Be(_nonApprovedStandardDetails.TrailblazerContact);
            result.OverviewOfRole.Should().Be(_nonApprovedStandardDetails.OverviewOfRole);
            result.StandardPageUrl.Should().Be(_nonApprovedStandardDetails.StandardPageUrl);
            result.StandardLevel.Should().Be(_nonApprovedStandardDetails.Level);
        }

        [Test]
        public async Task And_LevelIsZero_Then_LevelIsToBeConfirmed()
        {
            _nonApprovedStandardDetails.Level = "0";

            var result = await _getNonApprovedStandardDetails.Handle(_request, CancellationToken.None);

            result.StandardLevel.Should().Be("To be confirmed");
        }
    }
}
