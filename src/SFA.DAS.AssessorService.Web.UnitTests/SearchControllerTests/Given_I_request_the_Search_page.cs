using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Web.UnitTests.SearchControllerTests
{
    [TestFixture]
    public class Given_I_request_the_Search_page : SearchControllerTestBase
    {
        [Test]
        public void Then_I_receive_the_Search_page()
        {
            var result = SearchController.Index();
            result.Should().BeOfType<ViewResult>();
            ((ViewResult) result).ViewName.Should().Be("Index");
        }
    }
}