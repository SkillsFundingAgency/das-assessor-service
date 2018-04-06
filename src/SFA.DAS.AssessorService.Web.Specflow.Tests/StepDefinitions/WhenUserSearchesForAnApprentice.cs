using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers;
using SFA.DAS.AssessorService.Web.Specflow.Tests.Pages;
using SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.StepDefinitions
{
    [Binding]
    public class WhenUserSearchesForAnApprentice : BaseTest
    {
        private SearchForAnApprenticePage _searchForAnApprenticePage;
    
        private readonly By _surNameBy = By.Name("Surname");
        private readonly By _ulnBy = By.Name("Uln");
        private readonly By _searchButtonBy = By.Id("button-search");
    
        [Given(@"User should be navigated to search for an apprentice page on EPAO service")]
        public void GivenUserShouldBeNavigatedToSearchForAnApprenticePageOnEPAOService()
        {
            _searchForAnApprenticePage = new SearchForAnApprenticePage(webDriver);
        }

        [Given(@"User enters valid search criteria")]
        public void GivenUserEntersValidSearchCriteria(IEnumerable<dynamic> searchCriterias)
        {
            var searchCriteria = searchCriterias.First();
            string surname = searchCriteria.lastname;
            string uln = searchCriteria.ULN.ToString();

            FormCompletionHelper.EnterText(_surNameBy, surname);
            FormCompletionHelper.EnterText(_ulnBy, uln);
        }

        [Given(@"Clicks On Search Apprentice Button")]
        public void WhenIClickOnTheButton()
        {
            FormCompletionHelper.ClickElement(_searchButtonBy);
        }
    }
}
