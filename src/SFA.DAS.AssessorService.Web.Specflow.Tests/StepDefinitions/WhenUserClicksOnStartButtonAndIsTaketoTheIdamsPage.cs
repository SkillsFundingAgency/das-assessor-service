using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserClicksOnStartButtonAndIsTaketoTheIdamsPage : BaseTest
    {
        [Given(@"I have already registered on idAMS")]
        public void GivenIHaveAlreadyRegisteredOnIdAMS()
        {
        }

        [Given(@"I'm on the record end point assessment outcome page")]
        public void GivenImOnTheRecordEndPointAssessmentOutcomePage()
        {
            webDriver.Url = Configurator.GetConfiguratorInstance().GetBaseUrl();
        }

        [When(@"I click on start now button on the record end point assessment outcome page")]
        public void WhenIClickOnStartNowButtonOnTheRecordEndPointAssessmentOutcomePage()
        {
            var recordEndPointAssessmentOutcomePage = new RecordEndPointAssessmentOutcomePage(webDriver);
            recordEndPointAssessmentOutcomePage.ClickStartNowButton();

        }

        [Then(@"I should be taken onto idMAS sign in page")]
        public void ThenIShouldBeTakenOntoIdMASSignInPage()
        {
            var idamsLogonPage = new IdamsLogonPage(webDriver);
        }
    }
}
