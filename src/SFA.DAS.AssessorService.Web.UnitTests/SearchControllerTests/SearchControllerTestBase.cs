using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Search;

namespace SFA.DAS.AssessorService.Web.UnitTests.SearchControllerTests
{
    public class SearchControllerTestBase
    {
        public SearchController SearchController { get; private set; }

        [SetUp]
        public void Arrange()
        {
            var searchApiClient = new Mock<ISearchApiClient>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(ca => ca.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")).Returns(new Claim("", "12345"));
            contextAccessor.Setup(ca => ca.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Returns(new Claim("", "username"));

            var mock = new Mock<ISession>();
            contextAccessor.SetupGet(ca => ca.HttpContext.Session).Returns(mock.Object);

            searchApiClient.Setup(api =>
                    api.Search(It.Is<SearchQuery>(query => query.Surname == "Gouge" && query.Uln == 1234567890)))
                .ReturnsAsync(new List<SearchResult>() {new SearchResult() {FamilyName = "Gouge", Uln = 1234567890}});

            searchApiClient.Setup(api => api.Search(It.Is<SearchQuery>(query => query.Surname == "Smith" && query.Uln == 7777777777)))
                .ReturnsAsync(new List<SearchResult>() { });

            var orchestrator = new SearchOrchestrator(new Mock<ILogger<SearchController>>().Object, searchApiClient.Object,
                contextAccessor.Object);

            SearchController = new SearchController(orchestrator, new Mock<ISessionService>().Object);
        }
    }
}