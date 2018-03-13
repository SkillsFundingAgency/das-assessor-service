using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.UnitTests.SearchControllerTests
{
    [TestFixture]
    public class Given_I_enter_valid_values_in_the_ULN_and_Last_Name_fields : SearchControllerTestBase
    {
        [Test]
        [Ignore("")]
        public void Then_I_should_be_redirected_to_the_Search_Results_page()
        {
            var result = SearchController.Index(new SearchViewModel() {Surname = "Gouge", Uln = "1234567890"}).Result;
            result.Should().BeOfType<ViewResult>();

            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("Results");
        }

        [Test]
        [Ignore("")]
        public void Then_the_Search_Page_should_contain_apprentice_details()
        {
            var result = SearchController.Index(new SearchViewModel() { Surname = "Gouge", Uln = "1234567890" }).Result;
            
            var viewResult = result as ViewResult;
            viewResult.Model.Should().NotBeNull();

            var results = viewResult.Model as SearchViewModel;

            results.SearchResults.Should().NotBeNull();
            results.SearchResults.Count().Should().Be(1);
        }
    }
}