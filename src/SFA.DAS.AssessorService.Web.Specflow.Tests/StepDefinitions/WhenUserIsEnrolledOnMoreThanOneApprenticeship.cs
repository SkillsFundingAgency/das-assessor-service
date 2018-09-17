using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserIsEnrolledOnMoreThanOneApprenticeship : BaseTest
    {
        private AssessmentEnrolledOnMoreThanOneAssessmentPage _assessmentEnrolledOnMoreThanOneAssessmentPage;
        
        [Given(@"I'm on the Apprentice Select Apprentice Enrollment Page")]
        public void GivenIMOnTheApprenticeSelectApprenticeEnrollmentPage()
        {
            _assessmentEnrolledOnMoreThanOneAssessmentPage = new AssessmentEnrolledOnMoreThanOneAssessmentPage(webDriver);
        }

        [When(@"I Select a recording Assesmment")]
        public void WhenISelectARecordingAssesmment()
        {
            _assessmentEnrolledOnMoreThanOneAssessmentPage.SelectRecordingAssessment();
        }

        [When(@"I click on the Start Apprentice Enrollment Button")]
        public void WhenIClickOnTheStartApprenticeEnrollmentButton()
        {
            _assessmentEnrolledOnMoreThanOneAssessmentPage.ClickStartRecordingAssessment();
        }               
    }
}

