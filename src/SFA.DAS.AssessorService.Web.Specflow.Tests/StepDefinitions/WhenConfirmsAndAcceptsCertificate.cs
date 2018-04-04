using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenConfirmsAndAcceptsCertificate : BaseTest
    {
        private DeclarationtPage _declarationPage;
        private AssessmentRecodedPage _assessmentRecordedPage;

        [Then(@"The User is taken to the Declaraton Page")]
        public void ThenTheUserIsTakenToTheDeclaratonPage()
        {
            _declarationPage = new DeclarationtPage(webDriver);
        }

        [When(@"The User Conforms And Applies for a Certificate")]
        public void ThenTheUserConformsAndAppliesForACertificate()
        {
            _declarationPage.ClickConfirmAndAccept();
        }

        [Then(@"Assessment Is Recorded")]
        public void ThenAssessmentIsRecorded()
        {
            _assessmentRecordedPage = new AssessmentRecodedPage(webDriver);
            _assessmentRecordedPage.ClickSignOut();
        }
    }
}
