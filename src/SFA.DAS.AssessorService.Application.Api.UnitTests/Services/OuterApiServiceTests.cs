using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services
{
    public class OuterApiServiceTests
    {
        [Test]
        public async Task When_Fetching_Standard_Details_Then_Call_Outer_Api_For_Each_Standard()
        {
            var fixture = new Fixture();
            var uids = fixture.Create<IEnumerable<string>>();
            var outerApiClientMock = new Mock<IOuterApiClient>();
            var loggerMock = new Mock<ILogger<OuterApiService>>();

            var sut = new OuterApiService(outerApiClientMock.Object, loggerMock.Object);

            await sut.GetAllStandardDetails(uids);

            outerApiClientMock.Verify(o => o.Get<GetStandardByIdResponse>(It.IsAny<GetStandardByIdRequest>()), Times.Exactly(uids.Count()));
        }
    }
}
