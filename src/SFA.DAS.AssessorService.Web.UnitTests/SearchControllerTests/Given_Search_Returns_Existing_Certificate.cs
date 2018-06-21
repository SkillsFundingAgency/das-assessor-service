using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.UnitTests.SearchControllerTests
{
    [TestFixture]
    public class Given_Search_Returns_Existing_Certificate :SearchControllerTestBase
    {
        [Test]
        public void And_submitted_at_is_null_Then_ViewModel_Submitted_At_Is_Empty()
        {
            SearchOrchestrator.Setup(s =>
                    s.Search(It.Is<SearchRequestViewModel>(vm => vm.Surname == "Lamora" && vm.Uln == "1234567890")))
                .ReturnsAsync(new SearchRequestViewModel()
                {
                    SearchResults =
                        new List<ResultViewModel>() {new ResultViewModel() {FamilyName = "Lamora", Uln = "1234567890", SubmittedAt = null}}
                });

            SearchController.Index(new SearchRequestViewModel() {Surname = "Lamora", Uln = "1234567890"})
                .Wait();

            SessionService.Verify(ss => ss.Set("SelectedStandard", It.Is<SelectedStandardViewModel>(vm => vm.SubmittedAt == "")));
        }

        [Test]
        public void Then_ViewModel_Submitted_At_Does_Not_Include_Time()
        {
            SearchOrchestrator.Setup(s =>
                    s.Search(It.Is<SearchRequestViewModel>(vm => vm.Surname == "Lamora" && vm.Uln == "1234567890")))
                .ReturnsAsync(new SearchRequestViewModel()
                {
                    SearchResults =
                        new List<ResultViewModel>() { new ResultViewModel() { FamilyName = "Lamora", Uln = "1234567890", SubmittedAt = new DateTime(2018, 2, 3, 12,34,22) } }
                });

            SearchController.Index(new SearchRequestViewModel() { Surname = "Lamora", Uln = "1234567890" })
                .Wait();

            SessionService.Verify(ss => ss.Set("SelectedStandard", It.Is<SelectedStandardViewModel>(vm => vm.SubmittedAt == "3 February 2018")));
        }

        //[Test]
        //public void And_submitted_at_is_a_normal_date_Then_ViewModel_Submitted_At_Includes_time()
        //{
        //    SearchOrchestrator.Setup(s =>
        //            s.Search(It.Is<SearchRequestViewModel>(vm => vm.Surname == "Lamora" && vm.Uln == "1234567890")))
        //        .ReturnsAsync(new SearchRequestViewModel()
        //        {
        //            SearchResults =
        //                new List<ResultViewModel>() {new ResultViewModel() {FamilyName = "Lamora", Uln = "1234567890", SubmittedAt = new DateTime(2018,2,3,13,23,34)}}
        //        });

        //    SearchController.Index(new SearchRequestViewModel() {Surname = "Lamora", Uln = "1234567890"})
        //        .Wait();

        //    SessionService.Verify(ss => ss.Set("SelectedStandard", It.Is<SelectedStandardViewModel>(vm => vm.SubmittedAt == "3 February 2018 at 1:23pm")));
        //}
    }
}