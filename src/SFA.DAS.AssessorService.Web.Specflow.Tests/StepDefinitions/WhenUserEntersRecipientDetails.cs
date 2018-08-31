using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserEntersRecipientDetails : BaseTest
    {
        private RecipientDetailsPage _recipientDetailsPage;

        [Then(@"I should be taken to the Recipient Details Page")]
        public void ThenIShouldBeTakenToTheRecipientDetailsPage()
        {
            _recipientDetailsPage = new RecipientDetailsPage(webDriver);
        }

        [When(@"The recipient details are entered")]
        public void WhenTheRecipientDetailsAreEntered()
        {
            _recipientDetailsPage.EnterDetails();
        }

        [When(@"User clicks on the Recipient Continue Button")]
        public void WhenUserClicksOnTheRecipientContinueButton()
        {
            _recipientDetailsPage.ClicksContinue();
        }
    }
}
