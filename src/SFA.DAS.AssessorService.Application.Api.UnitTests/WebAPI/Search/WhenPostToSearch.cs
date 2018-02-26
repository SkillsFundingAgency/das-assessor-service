using FluentAssertions;
using Machine.Specifications;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.ExternalApis;
using SFA.DAS.AssessorService.ExternalApis.Types;
using SFA.DAS.AssessorService.ViewModel.Models;
using It = Moq.It;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Search
{
    [Subject("AssessorService")]
    public class WhenPostToSearch
    {
        private Establish context = () =>
        {
            _ilrApi = new Mock<IIlrApiClient>();
            _controller = new SearchController(_ilrApi.Object);
        };

        private Because of = () =>
        {
            _result = _controller.Search(new SearchQueryViewModel()).Result;
        };

        private Machine.Specifications.It Returns_A_Result = () =>
        {
            _result.Should().BeAssignableTo<IActionResult>();
        };

        private Machine.Specifications.It Calls_The_Ilr_Api = () =>
        {
            _ilrApi.Verify(api => api.Search(It.IsAny<SearchRequest>()));
        };

        private static SearchController _controller;
        private static object _result;
        private static Mock<IIlrApiClient> _ilrApi;
    }
}