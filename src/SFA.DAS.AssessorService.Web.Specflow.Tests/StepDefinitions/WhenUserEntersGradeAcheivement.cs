using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserEntersGradeAcheivement : BaseTest
    {
        private WhatGradeDidApprenticeAchievePage _whatGradeDidApprenticeAchievePage;
       
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
    }
}
