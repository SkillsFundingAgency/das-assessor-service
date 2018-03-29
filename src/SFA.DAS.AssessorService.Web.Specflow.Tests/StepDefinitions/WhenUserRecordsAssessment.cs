using System.Threading;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserRecordsAssessment
    {        
        [Given(@"User should be navigated to search for an apprentice page on EPAO service")]
        public void GivenUserShouldBeNavigatedToSearchForAnApprenticePageOnEPAOService()
        {
          
        }

        [Given(@"User enters valid search criteria")]
        public void GivenUserEntersValidSearchCriteria(Table table)
        {          
        }

        [Given(@"I'm on the ""(.*)"" page")]
        public void GivenImOnThePage(string p0)
        {         
            Thread.Sleep(10000);
        }

        [When(@"I click on the ""(.*)"" button")]
        public void WhenIClickOnTheButton(string p0)
        {         
        }

        [Then(@"I should be taken to the ""(.*)"" page")]
        public void ThenIShouldBeTakenToThePage(string p0)
        {         
        }
    }
}
