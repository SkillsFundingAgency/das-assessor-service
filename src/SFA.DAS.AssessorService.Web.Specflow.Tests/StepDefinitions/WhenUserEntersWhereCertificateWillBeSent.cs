using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserEntersWhereCertificateWillBeSent : BaseTest
    {
        private WhereWillTheCertificateBeSentPage _whereWillTheCertificateBeSentPage;

        [Then(@"I should be taken to the Where will the certificate be Sent Page")]
        public void ThenIShouldBeTakenToTheWhereWillTheCertificateBeSentPage()
        {
            _whereWillTheCertificateBeSentPage = new WhereWillTheCertificateBeSentPage(webDriver);
        }
                
        [When(@"I have entered Details for where to send the Certificate")]
        public void WhenIHaveEnteredDetailsForWhereToSendTheCertificate()
        {
            _whereWillTheCertificateBeSentPage.EnterDetails();
        }

        [When(@"User Clicks on Continue with the Certificate to be Sent Details")]
        public void WhenUserClicksOnContinueWithRgeCertificateToBeSentDetails()
        {
            _whereWillTheCertificateBeSentPage.ClickContinue();
        }
    }
}
