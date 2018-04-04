using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserSelectsAdditionalLearningOptions : BaseTest
    {
        private AddiitionalLearningOptionsPage _addiitionalLearningOptionsPage;
        private ApprenticeAchievementDatePage _apprenticeAchievementDatePage;

        [Then(@"User should be navigated to did the apprentice do any additional learning options page")]
        public void ThenUserShouldBeNavigatedToDidTheApprenticeDoAnyAdditionalLearningOptionsPage()
        {
            _addiitionalLearningOptionsPage = new AddiitionalLearningOptionsPage(webDriver);
        }

        [When(@"User Selects No Option")]
        public void WhenUserSelectsNoOption()
        {
            _addiitionalLearningOptionsPage.SelectsNoOption();
        }

        [When(@"User Clicks On Continue With Addtional Options")]
        public void WhenUserClicksOnContinueWithAddtionalOptions()
        {
            _addiitionalLearningOptionsPage.ClickContinue();
        }
    }
}
