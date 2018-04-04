using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.UnitTests.SearchControllerTests
{
    [TestFixture]
    public class Given_I_enter_invalid_values_in_the_ULN_and_Last_Name_fields : SearchControllerTestBase
    {
        [Test]
        public void Then_I_should_be_redirected_back_to_the_Search_page()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ResultViewModel, SearchResult>();
            });

            var result = SearchController.Index(new SearchRequestViewModel() { Surname = "Smith", Uln = "7777777777" }).Result;
            result.Should().BeOfType<ViewResult>();

            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("Index");
        }
    }
}