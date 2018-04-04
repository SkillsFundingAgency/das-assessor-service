using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserEntersAchievementDate : BaseTest
    {
        private ApprenticeAchievementDatePage _apprenticeAchievementDatePage;

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
    }
}
