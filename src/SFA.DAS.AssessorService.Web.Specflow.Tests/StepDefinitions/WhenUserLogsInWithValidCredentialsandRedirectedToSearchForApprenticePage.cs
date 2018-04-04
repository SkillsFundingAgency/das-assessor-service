using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserLogsInWithValidCredentialsandRedirectedToSearchForApprenticePage : BaseTest
    {
        private string _userName;
        private string _password;

        private SkillsFundingLogonPage _skillsFundingLogonPage;
        [Given(@"User enters valid credentials")]
        public void GivenUserEntersValidCredentials()
        {
            _userName = "isp\\testinge";
            _password = "Windmill1";

            _skillsFundingLogonPage = new SkillsFundingLogonPage(webDriver);
        }
      
        [When(@"Clicks on sign in button")]
        public void WhenClicksOnSignInButton()
        {
            _skillsFundingLogonPage.EnterUserDetails(_userName, _password);
        }

        [Given(@"Clicks on sign in button")]
        public void GivenClicksOnSignInButton()
        {
            _skillsFundingLogonPage.EnterUserDetails(_userName, _password);
        }

        [Then(@"User should be navigated to search for an apprentice page on EPAO service")]
        public void ThenUserShouldBeNavigatedToSearchForAnApprenticePageOnEPAOService()
        {
           
        }
    }
}

