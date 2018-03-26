using System;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace MyNamespace
{
    [Binding]
    public class WhenUserDoesNotHaveAnAccountIsRedirectedToDoesNotHaveAccountPage : BaseTest
    {
        private SkillsFundingLogonPage _skillsFundingLogonPage;

        [When(@"User Selects dont have an account")]
        public void WhenUserSelectsDontHaveAnAccount()
        {
            _skillsFundingLogonPage = new SkillsFundingLogonPage(webDriver);
            _skillsFundingLogonPage.SelectDontHaveAnAccount();
        }

        [Then(@"User should be navigated to dont have an account screen")]
        public void ThenUserShouldBeNavigatedToDontHaveAnAccountScreen()
        {
           
        }
    }
}
