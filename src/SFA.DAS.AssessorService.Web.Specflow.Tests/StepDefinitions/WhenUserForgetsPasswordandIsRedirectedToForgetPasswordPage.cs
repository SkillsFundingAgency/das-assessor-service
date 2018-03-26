using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class StepDefinitions : BaseTest
    {
        private SkillsFundingLogonPage _skillsFundingLogonPage;

        [When(@"User Selects forgot my password")]
        public void WhenUserSelectsForgotMyPassword()
        {
            _skillsFundingLogonPage = new SkillsFundingLogonPage(webDriver);
            _skillsFundingLogonPage.SelectForgotMyPassword();
        }

        [Then(@"User should be navigated to forgotten password screen")]
        public void ThenUserShouldBeNavigatedToForgottenPasswordScreen()
        {

        }
    }
}
