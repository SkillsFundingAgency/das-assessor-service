using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Search;
using SFA.DAS.AssessorService.Web.ViewModels.Search;
using System.Collections.Generic;
using System.Security.Claims;

namespace SFA.DAS.AssessorService.Web.UnitTests.SearchControllerTests
{
    public class SearchControllerTestBase
    {
        protected Mock<ISessionService> SessionService;
        protected Mock<ISearchOrchestrator> SearchOrchestrator;
        protected Mock<ISession> Session;
        public SearchController SearchController { get; private set; }

        [SetUp]
        public void Arrange()
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(ca => ca.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")).Returns(new Claim("", "12345"));
            contextAccessor.Setup(ca => ca.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Returns(new Claim("", "username"));

            Session = new Mock<ISession>();
            contextAccessor.SetupGet(ca => ca.HttpContext.Session).Returns(Session.Object);

            

            SearchOrchestrator = new Mock<ISearchOrchestrator>();

            SearchOrchestrator.Setup(s =>
                    s.Search(It.Is<SearchRequestViewModel>(vm => vm.Surname == "Gouge" && vm.Uln == "1234567890")))
                .ReturnsAsync(new SearchRequestViewModel()
                {
                    SearchResults =
                        new List<ResultViewModel>() {new ResultViewModel() {FamilyName = "Gouge", Uln = "1234567890"}}
                });

            SearchOrchestrator.Setup(s =>
                    s.Search(It.Is<SearchRequestViewModel>(vm => vm.Surname == "Smith" && vm.Uln == "7777777777")))
                .ReturnsAsync(new SearchRequestViewModel()
                {
                    SearchResults =
                        new List<ResultViewModel>()
                });
            

            SessionService = new Mock<ISessionService>();
            SearchController = new SearchController(SearchOrchestrator.Object, SessionService.Object);
        }
    }
}