using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;

namespace SFA.DAS.AssessorService.Web.UnitTests.SearchControllerTests
{
    [TestFixture]
    public class Given_I_enter_valid_details_search_result_returned : SearchControllerTestBase
    {
        [Test]
        public void And_standard_versions_are_populated()
        {
            SearchOrchestrator.Setup(s =>
                    s.Search(It.Is<SearchRequestViewModel>(vm => vm.Surname == "Lamora" && vm.Uln == "1234567890")))
                .ReturnsAsync(new SearchRequestViewModel()
                {
                    SearchResults =
                        new List<ResultViewModel>() 
                        { 
                            new ResultViewModel() { FamilyName = "Lamora", Uln = "1234567890", 
                                Versions = new List<StandardVersionViewModel> {
                                    new StandardVersionViewModel { StandardUId="StandardUId1",Version="1.0" },
                                    new StandardVersionViewModel { StandardUId="StandardUId2",Version="1.1" } } } 
                        }
                });

            SearchController.Index(new SearchRequestViewModel() { Surname = "Lamora", Uln = "1234567890" })
                .Wait();

            SessionService.Verify(ss => ss.Set("SelectedStandard", It.Is<SelectedStandardViewModel>(vm => 
                vm.Versions.Count() == 2
                && vm.Versions.First().StandardUId == "StandardUId1")));
        }
    }
}