using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserRecordsAssessment : BaseTest
    {
        private SearchForAnApprenticePage _searchForAnApprenticePage;
        private ConfirmApprenticePage _confirmApprenticePage;
        private WhatGradeDidApprenticeAchievePage _whatGradeDidApprenticeAchievePage;
        private AddiitionalLearningOptionsPage _addiitionalLearningOptionsPage;
        private ApprenticeAchievementDatePage _apprenticeAchievementDatePage;
        private WhereWillTheCertificateBeSentPage _whereWillTheCertificateBeSentPage;

        private By _surNameBy = By.Name("Surname");
        private By _ulnBy = By.Name("Uln");
        private By searchButtonBy = By.Id("button-search");
        private CheckAndApproveAssessmentDetails _checkAndApproveAssessmentDetails;

        [Given(@"User should be navigated to search for an apprentice page on EPAO service")]
        public void GivenUserShouldBeNavigatedToSearchForAnApprenticePageOnEPAOService()
        {
            _searchForAnApprenticePage = new SearchForAnApprenticePage(webDriver);
        }

        [Given(@"User enters valid search criteria")]
        public void GivenUserEntersValidSearchCriteria(IEnumerable<dynamic> searchCriterias)
        {
            var searchCriteria = searchCriterias.First();
            string surname = searchCriteria.lastname;
            string uln = searchCriteria.ULN.ToString();

            FormCompletionHelper.EnterText(_surNameBy, surname);
            FormCompletionHelper.EnterText(_ulnBy, uln);
        }


        [Given(@"Clicks On Search Apprentice Button")]
        public void WhenIClickOnTheButton()
        {
            FormCompletionHelper.ClickElement(searchButtonBy);
        }


        [Given(@"I'm on the Confirm Apprentice Page")]
        public void GivenImOnTheConfirmApprenticeage()
        {
            _confirmApprenticePage = new ConfirmApprenticePage(webDriver);
        }

        [When(@"I click on the Start Recording Assessment Button")]
        public void WhenIClickOnTheStartRecordingAssessmentButton()
        {
            _confirmApprenticePage.ClickStartRecordingAssessment();
        }

        [Then(@"I should be taken to the What Grade did the Apprentice Achieve page")]
        public void ThenIShouldBeTakenToTheWhatGradeDidTheApprenticeAchievePage()
        {
            _whatGradeDidApprenticeAchievePage = new WhatGradeDidApprenticeAchievePage(webDriver);
        }

        [When(@"The User Selects Grade")]
        public void WhenTheUserSelectsGrade()
        {
            _whatGradeDidApprenticeAchievePage.SelectsOption();
        }


        [When(@"User Clicks On Continue With What Grade")]
        public void WhenUserClicksOnContinueWithWhatGrade()
        {
            _whatGradeDidApprenticeAchievePage.ClicksContinue();
        }

        [Then(@"User should be navigated to did the apprentice do any additional learning options page")]
        public void ThenUserShouldBeNavigatedToDidTheApprenticeDoAnyAdditionalLearningOptionsPage()
        {
            _addiitionalLearningOptionsPage = new AddiitionalLearningOptionsPage(webDriver);
        }

        //[Then(@"User Clicks On Continue With Addtional Options")]
        //public void ThenUserClicksOnContinueWithAddtionalOptions()
        //{
        //    _addiitionalLearningOptionsPage.ClickContinue();
        //}


        [When(@"User Clicks On Continue With Addtional Options")]
        public void WhenUserClicksOnContinueWithAddtionalOptions()
        {
            _addiitionalLearningOptionsPage.ClickContinue();
        }

        [Then(@"User Is on the Apprentice Achievement Date Page")]
        public void ThenUserIsOnTheApprenticeAchievementDatePage()
        {
            _apprenticeAchievementDatePage = new ApprenticeAchievementDatePage(webDriver);
        }

        [When(@"The User Enters Detials Achievement Date")]
        public void WhenTheUserEntersDetialsAchievementDate()
        {
            _apprenticeAchievementDatePage.EnterDetails();
        }

        [When(@"Clicks On Continue with Apprentice Detials Achievement Date")]
        public void WhenClicksOnContinueWithApprenticeDetialsAchievementDate()
        {
            _apprenticeAchievementDatePage.ClickContinue();
        }

        [Then(@"I should be taken to the Where will the certificate be Sent Page")]
        public void ThenIShouldBeTakenToTheWhereWillTheCertificateBeSentPage()
        {
            _whereWillTheCertificateBeSentPage = new WhereWillTheCertificateBeSentPage(webDriver);
        }

        [When(@"I have entered Details for where to send the Certificate")]
        public void WhenIHaveEnteredDetailsForWhereToSendTheCertificate()
        {
            _whereWillTheCertificateBeSentPage.EnterDetails();
        }

        [When(@"User Clicks on Continue with rge Certificate to be Sent Details")]
        public void WhenUserClicksOnContinueWithRgeCertificateToBeSentDetails()
        {
            _whereWillTheCertificateBeSentPage.ClickContinue();
        }

        [Then(@"I should be navigated to the Check and Approve the Assessment Details Page")]
        public void ThenIShouldBeNavigatedToTheCheckAndApproveTheAssessmentDetailsPage()
        {
            _checkAndApproveAssessmentDetails = new CheckAndApproveAssessmentDetails(webDriver);
        }
    }
}
