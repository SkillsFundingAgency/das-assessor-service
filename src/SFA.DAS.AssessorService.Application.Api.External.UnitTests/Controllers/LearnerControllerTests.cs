using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.External.Controllers;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Learners;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.UnitTests.Controllers
{
    public class LearnerControllerTests
    {
        private GetBatchLearnerRequest _request;
        private GetLearnerResponse _response;

        private Mock<IApiClient> _mockApiClient;

        private LearnerController _learnerController;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _request = fixture.Create<GetBatchLearnerRequest>();

            _response = fixture.Build<GetLearnerResponse>()
                .With(r => r.ValidationErrors, new List<string>()).Create();

            _mockApiClient = new Mock<IApiClient>();
            _mockApiClient.Setup(c => c.GetLearner(It.IsAny<GetBatchLearnerRequest>())).ReturnsAsync(_response);

            _learnerController = new LearnerController(Mock.Of<ILogger<LearnerController>>(), Mock.Of<IHeaderInfo>(), _mockApiClient.Object);
        }

        [Test, MoqAutoData]
        public async Task When_RequestingLearner_AndLearnerFound_Then_ReturnLearnerWith200(long uln, string familyName, string standard)
        {
            var result = await _learnerController.GetLearner(uln, familyName, standard) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = result.Value as GetLearner;

            model.LearnerData.Learner.Should().BeEquivalentTo(_response.Learner.LearnerData.Learner);
        }

        [Test, MoqAutoData]
        public async Task When_RequestingLearner_AndValidationErrorReturned_Then_Return403(long uln, string familyName, string standard)
        {
            _response.ValidationErrors.Add("Validation Error");

            var result = await _learnerController.GetLearner(uln, familyName, standard) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
        }

        [Test, MoqAutoData]
        public async Task When_RequestingLearner_AndLearnerNotFound_Then_Return404(long uln, string familyName, string standard)
        {
            _response.Learner = null;

            var result = await _learnerController.GetLearner(uln, familyName, standard) as NotFoundResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
