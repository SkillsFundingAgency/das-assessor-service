using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserConfirmsApprentice : BaseTest
    {
        private ConfirmApprenticePage _confirmApprenticePage;
      
        [Then(@"I'm on the Confirm Apprentice Page")]
        public void ThenIMOnTheConfirmApprenticePage()
        {
            _confirmApprenticePage = new ConfirmApprenticePage(webDriver);
        }              

        [When(@"I click on the Start Recording Assessment Button")]
        public void WhenIClickOnTheStartRecordingAssessmentButton()
        {
            _confirmApprenticePage.ClickStartRecordingAssessment();
        }
    }
}
