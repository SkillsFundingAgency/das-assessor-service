using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Certificates.Query
{
    public class GetApprenticeLearner
    {
        private Mock<IMediator> _mockMediator;

        private LearnerDetailsQueryController _sut;

        [SetUp]
        public void Arrange()
        {
            _mockMediator = new Mock<IMediator>();

            _sut = new LearnerDetailsQueryController(_mockMediator.Object);
        }

        [Test, MoqAutoData]
        public async Task And_StandardCodeIsIncluded_Then_ShouldCallQuery(long apprenticeshipId)
        {
            var result = await _sut.GetApprenticeLearner(apprenticeshipId);

            _mockMediator.Verify(r => r.Send(It.Is<GetApprenticeLearnerRequest>(q => q.ApprenticeshipId == apprenticeshipId), It.IsAny<CancellationToken>()));
        }
    }
}
