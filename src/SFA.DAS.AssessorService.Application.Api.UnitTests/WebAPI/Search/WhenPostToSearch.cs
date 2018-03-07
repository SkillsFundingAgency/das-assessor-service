using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.Ilr;
using SFA.DAS.AssessorService.ExternalApis.Ilr.Types;


namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Search
{
    //using AssessorService.Api.Types.Models;

    //[TestFixture]
    //public class WhenPostToSearch
    //{
    //    [SetUp]
    //    public void Arrange()
    //    {
    //        _ilrApi = new Mock<IIlrApiClient>();
    //        _assessmentOrgsApi = new Mock<IAssessmentOrgsApiClient>();
    //        _orgRepository = new Mock<IOrganisationQueryRepository>();
    //        _controller = new SearchController(_ilrApi.Object, _assessmentOrgsApi.Object, _orgRepository.Object);
    //    }

    //    [Test]
    //    public void Returns_A_Result()
    //    {
    //        _result = _controller.Search(new SearchQuery()).Result;
    //        _result.Should().BeAssignableTo<IActionResult>();
    //    }

    //    [Test]
    //    public void Calls_The_Ilr_Api()
    //    {
    //        _result = _controller.Search(new SearchQuery()).Result;
    //        _ilrApi.Verify(api => api.Search(It.IsAny<IlrSearchRequest>()));
    //    }

    //    private static SearchController _controller;
    //    private static object _result;
    //    private static Mock<IIlrApiClient> _ilrApi;
    //    private Mock<IAssessmentOrgsApiClient> _assessmentOrgsApi;
    //    private Mock<IOrganisationQueryRepository> _orgRepository;
    //}
}