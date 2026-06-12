using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;

namespace SFA.DAS.AssessorService.Web.UnitTests.SearchControllerTests
{
    [TestFixture]
    public class Given_I_enter_valid_search_details : SearchControllerTestBase
    {
        private SearchRequestViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _viewModel = new SearchRequestViewModel { SearchResults = null, Surname = "Lamora", Uln = "1234567890" };
        }

        [Test]
        public void Then_results_are_returned_And_standard_versions_are_populated()
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

        [Test]
        public async Task And_there_are_multiple_potential_standards_Then_redirect_to_ChooseStandard()
        {
            SearchOrchestrator.Setup(s => s.Search(It.Is<SearchRequestViewModel>(vm => vm.Surname == "Lamora" && vm.Uln == "1234567890")))
                .ReturnsAsync(AddSearchResultsTwoStandards);

            var result = await SearchController.Index(_viewModel) as RedirectToActionResult;

            result.ActionName.Should().Be("ChooseStandard");
        }

        [Test]
        public async Task And_there_are_draft_and_privately_funded_certifications_Then_redirect_to_PrivatelyFundedDraft()
        {
            SearchOrchestrator.Setup(s =>
                    s.Search(It.Is<SearchRequestViewModel>(vm => vm.Surname == "Lamora" && vm.Uln == "1234567890")))
                .ReturnsAsync(new SearchRequestViewModel()
                {
                    SearchResults =
                        new List<ResultViewModel>()
                        {
                            new ResultViewModel() { FamilyName = "Lamora", Uln = "1234567890",
                                CertificateStatus = CertificateStatus.Draft,
                                IsPrivatelyFunded = true
                            }
                        }
                });

            var result = await SearchController.Index(new SearchRequestViewModel() { Surname = "Lamora", Uln = "1234567890" }) as RedirectToActionResult;

            result.ActionName.Should().Be("PrivatelyFundedDraft");
        }

        private SearchRequestViewModel AddSearchResultsTwoStandards()
        {
            _viewModel.SearchResults = new List<ResultViewModel>()
            {
                new ResultViewModel()
                {
                    FamilyName = "Lamora",
                    Uln = "1234567890",
                    StandardReferenceNumber = "ST0001",
                    Versions = new List<StandardVersionViewModel>
                    {
                        new StandardVersionViewModel { StandardUId="StandardUId1", Version="1.0" },
                        new StandardVersionViewModel { StandardUId="StandardUId2", Version="1.1" }
                    }
                },
                new ResultViewModel()
                {
                    FamilyName = "Lamora",
                    Uln = "1234567890",
                    StandardReferenceNumber = "ST0003",
                    Versions = new List<StandardVersionViewModel>
                    {
                        new StandardVersionViewModel { StandardUId="StandardUId3",Version="1.0" }
                    }
                }
            };

            return _viewModel;
        }
    }
}