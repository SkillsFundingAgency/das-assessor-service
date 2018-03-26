using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class UserLogsInSuccesfully : BaseTest
    {
        [Given(@"User Logs on through idams")]
        public void GivenUserLogsOnThroughIdams()
        {
            webDriver.Url = Configurator.GetConfiguratorInstance().GetBaseUrl();
            var recordEndPointAssessmentOutcomePage = new RecordEndPointAssessmentOutcomePage(webDriver);
            var idamsLogonPage = recordEndPointAssessmentOutcomePage.ClickStartNowButton();
            idamsLogonPage.SelectProvider();
        }
    }
}
