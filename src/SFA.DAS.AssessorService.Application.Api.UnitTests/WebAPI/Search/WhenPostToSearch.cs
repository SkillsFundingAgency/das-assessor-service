using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.ExternalApis.Ilr;
using SFA.DAS.AssessorService.ExternalApis.Ilr.Types;


namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Search
{
    using AssessorService.Api.Types.Models;

    [TestFixture]
    public class WhenPostToSearch
    {
        [SetUp]
        public void Arrange()
        {
            _ilrApi = new Mock<IIlrApiClient>();
            _controller = new SearchController(_ilrApi.Object);
        }

        [Test]
        public void Returns_A_Result()
        {
            _result = _controller.Search(new SearchQueryViewModel()).Result;
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void Calls_The_Ilr_Api()
        {
            _result = _controller.Search(new SearchQueryViewModel()).Result;
            _ilrApi.Verify(api => api.Search(It.IsAny<SearchRequest>()));
        }

        private static SearchController _controller;
        private static object _result;
        private static Mock<IIlrApiClient> _ilrApi;
    }
}