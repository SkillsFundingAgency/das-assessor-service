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

            searchApiClient.Setup(api => api.Search(It.Is<SearchQuery>(query => query.Surname == "Gouge" && query.Uln == "1234567890")))
                .ReturnsAsync(new SearchResult() { Results = new List<Result>() { new Result() { Surname = "Gouge", Uln = "1234567890" } }.AsEnumerable() });

            searchApiClient.Setup(api => api.Search(It.Is<SearchQuery>(query => query.Surname == "Smith" && query.Uln == "7777777777")))
                .ReturnsAsync(new SearchResult() { Results = new List<Result>() { }.AsEnumerable() });

            SearchController = new SearchController(new Mock<ILogger<SearchController>>().Object,
                searchApiClient.Object, contextAccessor.Object);
        }
    }
}