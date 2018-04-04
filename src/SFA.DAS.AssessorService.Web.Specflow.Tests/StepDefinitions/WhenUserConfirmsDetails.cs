using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserConfirmsDetails : BaseTest
    {
        private CheckAndApproveAssessmentDetails _checkAndApproveAssessmentDetails;

        [Then(@"I should be navigated to the Check and Approve the Assessment Details Page")]
        public void ThenIShouldBeNavigatedToTheCheckAndApproveTheAssessmentDetailsPage()
        {
            _checkAndApproveAssessmentDetails = new CheckAndApproveAssessmentDetails(webDriver);
        }

        [When(@"User Confirms Check And Approve Details")]
        public void WhenUserConfirmsCheckAndApproveDetails()
        {
            _checkAndApproveAssessmentDetails.ClickContinue();
        }
    }
}
